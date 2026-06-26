using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // 🔥 GET SCOPED SERVICE CORRECTLY
                var errorRepo = context.RequestServices.GetRequiredService<ErrorRepository>();

                var errorId = Guid.NewGuid().ToString("N");
                var fullMessage = $"ErrorId: {errorId}\n{ex}";

                await errorRepo.LogErrorAsync("GlobalExceptionMiddleware", fullMessage);

                var clientMessage = _env.IsDevelopment()
                    ? $"An error occurred (Ref: {errorId}). Details: {ex.Message}"
                    : $"Unexpected error. Reference Id: {errorId}";

                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = "application/json";

                var response = new ApiResponse<string>
                {
                    Status = false,
                    Code = "500",
                    Message = clientMessage,
                    Data = null
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
