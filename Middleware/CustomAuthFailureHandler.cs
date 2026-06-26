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

            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = "application/json";

            var json = JsonSerializer.Serialize(response);
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

            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = "application/json";

            var json = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(json);
        }
    }
}
