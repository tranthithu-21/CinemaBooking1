using CoreBusiness;
using Dapper;
using Microsoft.Data.SqlClient;
using UseCases.DataStorePluginInterfaces;

namespace Plugins.DataStore.SqlServer
{
    public class AuditoriumSqlServerRepository : IAuditoriumRepository
    {
        private readonly string _connectionString;

        public AuditoriumSqlServerRepository(SqlServerConfiguration config)
        {
            _connectionString = config.ConnectionString;
        }

        public async Task<IEnumerable<Auditorium>> GetAllAuditoriumsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
                SELECT AuditoriumId, Name, TotalSeats 
                FROM dbo.Auditoriums 
                ORDER BY AuditoriumId";
            return await connection.QueryAsync<Auditorium>(sql);
        }

        public async Task<Auditorium?> GetAuditoriumByIdAsync(int auditoriumId)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
                SELECT AuditoriumId, Name, TotalSeats 
                FROM dbo.Auditoriums 
                WHERE AuditoriumId = @AuditoriumId";
            return await connection.QueryFirstOrDefaultAsync<Auditorium>(sql, new { AuditoriumId = auditoriumId });
        }
    }
}
