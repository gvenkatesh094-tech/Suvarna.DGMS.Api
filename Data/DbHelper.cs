using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Suvarna.DGMS.Api.Data
{
    public class DbHelper
    {
        private readonly string _connectionString;

        public DbHelper(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("SqlServer")
                ?? throw new InvalidOperationException("Connection string 'SqlServer' is missing.");
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // INSERT / UPDATE / DELETE
        public int ExecuteNonQuery(string sql, object? param = null)
        {
            using var conn = GetConnection();
            return conn.Execute(sql, param);
        }

        // Single row
        public T? GetSingle<T>(string sql, object? param = null)
        {
            using var conn = GetConnection();
            return conn.QueryFirstOrDefault<T>(sql, param);
        }

        // Multiple rows
        public List<T> GetList<T>(string sql, object? param = null)
        {
            using var conn = GetConnection();
            return conn.Query<T>(sql, param).ToList();
        }
    }
}