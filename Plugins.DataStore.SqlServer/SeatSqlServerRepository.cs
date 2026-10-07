using CoreBusiness;
using Dapper;
using Microsoft.Data.SqlClient;
using UseCases.DataStorePluginInterfaces;

namespace Plugins.DataStore.SqlServer
{
    public class SeatSqlServerRepository : ISeatRepository
    {
        private readonly string _connectionString;

        public SeatSqlServerRepository(SqlServerConfiguration config)
        {
            _connectionString = config.ConnectionString;
        }

        public async Task<IEnumerable<Seat>> GetSeatsByAuditoriumIdAsync(int auditoriumId)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
                SELECT SeatId, AuditoriumId, SeatNumber, [Row], Number, SeatType 
                FROM dbo.Seats 
                WHERE AuditoriumId = @AuditoriumId 
                ORDER BY [Row], Number";
            return await connection.QueryAsync<Seat>(sql, new { AuditoriumId = auditoriumId });
        }

        public async Task<IEnumerable<int>> GetBookedSeatIdsByShowtimeIdAsync(int showtimeId)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
                SELECT SeatId 
                FROM dbo.Tickets 
                WHERE ShowtimeId = @ShowtimeId";
            return await connection.QueryAsync<int>(sql, new { ShowtimeId = showtimeId });
        }

        public async Task<Seat?> GetSeatByIdAsync(int seatId)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
                SELECT SeatId, AuditoriumId, SeatNumber, [Row], Number, SeatType 
                FROM dbo.Seats 
                WHERE SeatId = @SeatId";
            return await connection.QueryFirstOrDefaultAsync<Seat>(sql, new { SeatId = seatId });
        }
    }
}
