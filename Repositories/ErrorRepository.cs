using Npgsql;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Suvarna.DGMS.Api.Repositories
{
    public class ErrorRepository
    {
        private readonly string _connectionString;

        public ErrorRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("Postgres");
        }

        public async Task LogErrorAsync(string procedure, string message)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    using (var cmd = new NpgsqlCommand("pr_log_error_sp", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("p_procedure_name", procedure);
                        cmd.Parameters.AddWithValue("p_error_message", message);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch
            {
                // Never throw from logger
            }
        }
    }
}
