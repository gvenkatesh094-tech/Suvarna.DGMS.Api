using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            var method = context.Request.Method;
            var path = context.Request.Path;

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                int statusCode = context.Response.StatusCode;

                // 🔥 Log to console or file
                _logger.LogInformation(
                    "HTTP {Method} {Path} responded {StatusCode} in {Duration} ms",
                    method,
                    path,
                    statusCode,
                    stopwatch.ElapsedMilliseconds
                );

                // OPTIONAL: Log to DB (SCOPED SERVICE)
                var errorRepo = context.RequestServices.GetRequiredService<ErrorRepository>();

                string message = $"[{method}] {path} => {statusCode} ({stopwatch.ElapsedMilliseconds} ms)";

                await errorRepo.LogErrorAsync("RequestLogging", message);
            }
        }
    }
}
