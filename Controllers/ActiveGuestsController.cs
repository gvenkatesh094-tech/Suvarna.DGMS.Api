using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/activeguests")]
    public class ActiveGuestsController : ControllerBase
    {
        private readonly ActiveGuestsService _service;

        public ActiveGuestsController(ActiveGuestsService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetActiveGuests([FromQuery] int sessionId, [FromQuery] int? propertyId = null)
        {
            return Ok(_service.GetActiveGuests(sessionId, propertyId));
        }
    }
}
