using CoreBusiness;
using Dapper;
using Microsoft.Data.SqlClient;
using UseCases.DataStorePluginInterfaces;

namespace Plugins.DataStore.SqlServer
{
    public class ShowtimeSqlServerRepository : IShowtimeRepository
    {
        private readonly string _connectionString;

        public ShowtimeSqlServerRepository(SqlServerConfiguration config)
        {
            _connectionString = config.ConnectionString;
        }

        public async Task<IEnumerable<Showtime>> GetAllShowtimesAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
                SELECT s.ShowtimeId, s.MovieId, s.AuditoriumId, s.StartTime, s.EndTime, s.Price,
                       m.MovieId, m.Title, m.DurationMinutes, m.PosterUrl, m.Genre,
                       a.AuditoriumId, a.Name, a.TotalSeats
                FROM dbo.Showtimes s
                INNER JOIN dbo.Movies m ON s.MovieId = m.MovieId
                INNER JOIN dbo.Auditoriums a ON s.AuditoriumId = a.AuditoriumId
                ORDER BY s.StartTime ASC";

            return await connection.QueryAsync<Showtime, Movie, Auditorium, Showtime>(
                sql,
                (showtime, movie, auditorium) =>
                {
                    showtime.Movie = movie;
                    showtime.Auditorium = auditorium;
                    return showtime;
                },
                splitOn: "MovieId,AuditoriumId");
        }

        public async Task<IEnumerable<Showtime>> GetShowtimesByMovieIdAsync(int movieId)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
                SELECT s.ShowtimeId, s.MovieId, s.AuditoriumId, s.StartTime, s.EndTime, s.Price,
                       m.MovieId, m.Title, m.DurationMinutes, m.PosterUrl, m.Genre,
                       a.AuditoriumId, a.Name, a.TotalSeats
                FROM dbo.Showtimes s
                INNER JOIN dbo.Movies m ON s.MovieId = m.MovieId
                INNER JOIN dbo.Auditoriums a ON s.AuditoriumId = a.AuditoriumId
                WHERE s.MovieId = @MovieId
                ORDER BY s.StartTime ASC";

            return await connection.QueryAsync<Showtime, Movie, Auditorium, Showtime>(
                sql,
                (showtime, movie, auditorium) =>
                {
                    showtime.Movie = movie;
                    showtime.Auditorium = auditorium;
                    return showtime;
                },
                new { MovieId = movieId },
                splitOn: "MovieId,AuditoriumId");
        }

        public async Task<Showtime?> GetShowtimeByIdAsync(int showtimeId)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
                SELECT s.ShowtimeId, s.MovieId, s.AuditoriumId, s.StartTime, s.EndTime, s.Price,
                       m.MovieId, m.Title, m.DurationMinutes, m.PosterUrl, m.Genre,
                       a.AuditoriumId, a.Name, a.TotalSeats
                FROM dbo.Showtimes s
                INNER JOIN dbo.Movies m ON s.MovieId = m.MovieId
                INNER JOIN dbo.Auditoriums a ON s.AuditoriumId = a.AuditoriumId
                WHERE s.ShowtimeId = @ShowtimeId";

            var list = await connection.QueryAsync<Showtime, Movie, Auditorium, Showtime>(
                sql,
                (showtime, movie, auditorium) =>
                {
                    showtime.Movie = movie;
                    showtime.Auditorium = auditorium;
                    return showtime;
                },
                new { ShowtimeId = showtimeId },
                splitOn: "MovieId,AuditoriumId");

            return list.FirstOrDefault();
        }

        public async Task<int> AddShowtimeAsync(Showtime showtime)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
                INSERT INTO dbo.Showtimes (MovieId, AuditoriumId, StartTime, EndTime, Price)
                VALUES (@MovieId, @AuditoriumId, @StartTime, @EndTime, @Price);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            return await connection.ExecuteScalarAsync<int>(sql, new
            {
                showtime.MovieId,
                showtime.AuditoriumId,
                showtime.StartTime,
                showtime.EndTime,
                showtime.Price
            });
        }

        public async Task UpdateShowtimeAsync(Showtime showtime)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
                UPDATE dbo.Showtimes
                SET MovieId = @MovieId,
                    AuditoriumId = @AuditoriumId,
                    StartTime = @StartTime,
                    EndTime = @EndTime,
                    Price = @Price
                WHERE ShowtimeId = @ShowtimeId";

            await connection.ExecuteAsync(sql, new
            {
                showtime.ShowtimeId,
                showtime.MovieId,
                showtime.AuditoriumId,
                showtime.StartTime,
                showtime.EndTime,
                showtime.Price
            });
        }

        public async Task DeleteShowtimeAsync(int showtimeId)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
                DELETE FROM dbo.Showtimes 
                WHERE ShowtimeId = @ShowtimeId";

            await connection.ExecuteAsync(sql, new { ShowtimeId = showtimeId });
        }

        /// <summary>
        /// Phục vụ LUẬT NGHIỆP VỤ 2 (K1.3): Kiểm tra trùng lịch phòng chiếu.
        /// Sử dụng 100% Parameterized query: không nối chuỗi (K2.3).
        /// </summary>
        public async Task<bool> HasOverlapAsync(int auditoriumId, DateTime startTime, DateTime endTime, int? excludeShowtimeId = null)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
                SELECT COUNT(1) 
                FROM dbo.Showtimes 
                WHERE AuditoriumId = @AuditoriumId 
                  AND (@ExcludeShowtimeId IS NULL OR ShowtimeId <> @ExcludeShowtimeId)
                  AND StartTime < @EndTime 
                  AND EndTime > @StartTime";

            var count = await connection.ExecuteScalarAsync<int>(sql, new
            {
                AuditoriumId = auditoriumId,
                StartTime = startTime,
                EndTime = endTime,
                ExcludeShowtimeId = excludeShowtimeId
            });

            return count > 0;
        }
    }
}
