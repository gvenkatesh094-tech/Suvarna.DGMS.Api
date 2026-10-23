using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    /// <summary>Permission screen: which user can use which location / property.</summary>
    [Authorize]
    [ApiController]
    [Route("api/user-access")]
    public class UserAccessController : ControllerBase
    {
        private readonly UserAccessService _service;

        public UserAccessController(UserAccessService service)
        {
            _service = service;
        }

        [HttpGet("users")]
        public async Task<IActionResult> Users([FromQuery] string? search)
            => Ok(await _service.GetUsers(search));

        [HttpGet("properties")]
        public async Task<IActionResult> Properties()
            => Ok(await _service.GetProperties());

        [HttpGet("user/{userId:int}")]
        public async Task<IActionResult> UserAccess(int userId)
            => Ok(await _service.GetUserAccess(userId));

        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] SaveUserPropertyAccessRequest request)
            => Ok(await _service.Save(request));
    }
}
