using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Suvarna.DGMS.Api.Models;

namespace Suvarna.DGMS.Api.Middleware
{
    public class CustomAuthFailureHandler : JwtBearerEvents
    {
        // The Angular app reads lower-case fields (status, code, message, data).
        // JsonSerializer.Serialize() defaults to PascalCase, so camelCase is set explicitly.
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public override Task Challenge(JwtBearerChallengeContext context)
        {
            context.HandleResponse();

            var response = new ApiResponse<string>
            {
                Status = false,
                Code = "401",
                Message = "Unauthorized access. Invalid or missing token.",
                Data = null
            };

            // FIX: real HTTP 401. It used to be 200, so the browser could never tell
            // "token expired" from a normal answer and the auto token-refresh could not run.
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var json = JsonSerializer.Serialize(response, JsonOptions);
            return context.Response.WriteAsync(json);
        }

        public override Task Forbidden(ForbiddenContext context)
        {
            var response = new ApiResponse<string>
            {
                Status = false,
                Code = "403",
                Message = "Access denied. You do not have permission.",
                Data = null
            };

            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var json = JsonSerializer.Serialize(response, JsonOptions);
            return context.Response.WriteAsync(json);
        }
    }
}
