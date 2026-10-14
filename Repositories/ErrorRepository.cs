using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Suvarna.DGMS.Api.Repositories
{
    public class ErrorRepository
    {
        private readonly ILogger<ErrorRepository> _logger;

        public ErrorRepository(ILogger<ErrorRepository> logger)
        {
            _logger = logger;
        }

        // Writes to the log file instead of the database.
        // No network call, so it never slows down a request.
        public Task LogErrorAsync(string procedure, string message)
        {
            try
            {
                _logger.LogError("[{Procedure}] {Message}", procedure, message);
            }
            catch
            {
                // Never throw from logger
            }

            return Task.CompletedTask;
        }
    }
}