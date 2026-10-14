using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

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
                long duration = stopwatch.ElapsedMilliseconds;

                // Log to console / file only. No database call here,
                // so logging never slows down the API response.
                if (duration > 3000)
                {
                    _logger.LogWarning(
                        "SLOW REQUEST: HTTP {Method} {Path} responded {StatusCode} in {Duration} ms",
                        method, path, statusCode, duration);
                }
                else
                {
                    _logger.LogInformation(
                        "HTTP {Method} {Path} responded {StatusCode} in {Duration} ms",
                        method, path, statusCode, duration);
                }
            }
        }
    }
}