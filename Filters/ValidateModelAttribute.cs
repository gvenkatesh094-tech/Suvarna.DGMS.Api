using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Suvarna.DGMS.Api.Models;
using System.Linq;

namespace Suvarna.DGMS.Api.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(x => x.Value.Errors.Any())
                    .Select(x => new
                    {
                        Field = x.Key,
                        Error = x.Value.Errors.First().ErrorMessage
                    })
                    .ToList();

                var response = new ApiResponse<object>
                {
                    Status = false,
                    Code = "400",
                    Message = "Validation failed",
                    Data = errors
                };

                context.Result = new OkObjectResult(response);
            }
        }
    }
}
