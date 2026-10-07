namespace Plugins.DataStore.SqlServer
{
    public class SqlServerConfiguration
    {
        public string ConnectionString { get; set; } = string.Empty;

        public SqlServerConfiguration(string connectionString)
        {
            ConnectionString = connectionString;
        }
    }
}
