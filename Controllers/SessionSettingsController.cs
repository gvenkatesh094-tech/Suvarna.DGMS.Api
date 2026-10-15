using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Suvarna.DGMS.Api.Models;

namespace Suvarna.DGMS.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/session-settings")]
    public class SessionSettingsController : Controller
    {
        private readonly SessionSettings _settings;

        public SessionSettingsController(IOptions<SessionSettings> options)
        {
            _settings = options.Value;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                IdleTimeoutMinutes =
                    _settings.IdleTimeoutMinutes
            });
        }
    }
}
