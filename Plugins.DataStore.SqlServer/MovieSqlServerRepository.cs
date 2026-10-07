using CoreBusiness;
using Dapper;
using Microsoft.Data.SqlClient;
using UseCases.DataStorePluginInterfaces;

namespace Plugins.DataStore.SqlServer
{
    public class MovieSqlServerRepository : IMovieRepository
    {
        private readonly string _connectionString;

        public MovieSqlServerRepository(SqlServerConfiguration config)
        {
            _connectionString = config.ConnectionString;
        }

        public async Task<IEnumerable<Movie>> GetAllMoviesAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
                SELECT MovieId, Title, Description, DurationMinutes, PosterUrl, Genre, ReleaseDate 
                FROM dbo.Movies 
                ORDER BY ReleaseDate DESC";
            return await connection.QueryAsync<Movie>(sql);
        }

        public async Task<Movie?> GetMovieByIdAsync(int movieId)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
                SELECT MovieId, Title, Description, DurationMinutes, PosterUrl, Genre, ReleaseDate 
                FROM dbo.Movies 
                WHERE MovieId = @MovieId";
            return await connection.QueryFirstOrDefaultAsync<Movie>(sql, new { MovieId = movieId });
        }

        public async Task<int> AddMovieAsync(Movie movie)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
                INSERT INTO dbo.Movies (Title, Description, DurationMinutes, PosterUrl, Genre, ReleaseDate)
                VALUES (@Title, @Description, @DurationMinutes, @PosterUrl, @Genre, @ReleaseDate);
                SELECT CAST(SCOPE_IDENTITY() as int);";
            return await connection.ExecuteScalarAsync<int>(sql, new
            {
                movie.Title,
                movie.Description,
                movie.DurationMinutes,
                movie.PosterUrl,
                movie.Genre,
                movie.ReleaseDate
            });
        }

        public async Task UpdateMovieAsync(Movie movie)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
                UPDATE dbo.Movies 
                SET Title = @Title,
                    Description = @Description,
                    DurationMinutes = @DurationMinutes,
                    PosterUrl = @PosterUrl,
                    Genre = @Genre,
                    ReleaseDate = @ReleaseDate
                WHERE MovieId = @MovieId";
            await connection.ExecuteAsync(sql, new
            {
                movie.MovieId,
                movie.Title,
                movie.Description,
                movie.DurationMinutes,
                movie.PosterUrl,
                movie.Genre,
                movie.ReleaseDate
            });
        }

        public async Task DeleteMovieAsync(int movieId)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
                DELETE FROM dbo.Movies 
                WHERE MovieId = @MovieId";
            await connection.ExecuteAsync(sql, new { MovieId = movieId });
        }
    }
}
