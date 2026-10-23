using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;

        // camelCase so the Angular app can read status / code / message
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public GlobalExceptionMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        /// <summary>
        /// Text of a user-defined SQL error (number 50000 and above) anywhere in the exception chain;
        /// null for real errors (deadlock, timeout, truncation ...), which keep the "Reference Id" message.
        /// </summary>
        private static string? FindProcedureMessage(Exception ex)
        {
            for (Exception? e = ex; e != null; e = e.InnerException)
            {
                if (e is Microsoft.Data.SqlClient.SqlException sql)
                {
                    foreach (Microsoft.Data.SqlClient.SqlError err in sql.Errors)
                    {
                        if (err.Number >= 50000 && !string.IsNullOrWhiteSpace(err.Message))
                            return err.Message;
                    }
                    return null;
                }
            }
            return null;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Suvarna.DGMS.Api.Helpers.UserMessageException uex) when (!context.Response.HasStarted)
            {
                // NEW: business-rule message from a procedure (status 'E') - shown to the user as it is
                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = "application/json";

                var response = new ApiResponse<string>
                {
                    Status = false,
                    Code = "400",
                    Message = uex.Message,
                    Data = null
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
            }
            catch (Exception ex) when (!context.Response.HasStarted && FindProcedureMessage(ex) is string procMessage)
            {
                // NEW: a message raised on purpose by a procedure (RAISERROR / THROW 50000+, e.g.
                // "Building Code already exists.") is a business message - shown as it is, also in production.
                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = "application/json";

                var response = new ApiResponse<string>
                {
                    Status = false,
                    Code = "400",
                    Message = procMessage,
                    Data = null
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
            }
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid().ToString("N");
                var fullMessage = $"ErrorId: {errorId}\n{ex}";

                // FIX: if the error log itself fails (DB down), the user must still get a clean answer
                try
                {
                    var errorRepo = context.RequestServices.GetRequiredService<ErrorRepository>();
                    await errorRepo.LogErrorAsync("GlobalExceptionMiddleware", fullMessage);
                }
                catch (Exception logEx)
                {
                    Console.Error.WriteLine($"[{errorId}] Could not save error to DB: {logEx.Message}\n{ex}");
                }

                // Response already partly sent (e.g. file download) -> nothing more can be written
                if (context.Response.HasStarted)
                {
                    throw;
                }

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

                await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
            }
        }
    }
}
