using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/waitinglist")]
    public class WaitingListController : ControllerBase
    {
        private readonly WaitingListService _service;

        public WaitingListController(WaitingListService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetWaitlist([FromQuery] int sessionId)
        {
            return Ok(_service.GetWaitlist(sessionId));
        }

        [HttpPost("add")]
        public IActionResult AddToWaitlist([FromBody] AddWaitlistRequest request)
        {
            return Ok(_service.AddToWaitlist(request));
        }

        [HttpPost("{waitlistId:int}/notify")]
        public IActionResult Notify(int waitlistId, [FromQuery] int sessionId)
        {
            return Ok(_service.Notify(sessionId, waitlistId));
        }

        [HttpPost("{waitlistId:int}/remove")]
        public IActionResult Remove(int waitlistId, [FromQuery] int sessionId)
        {
            return Ok(_service.Remove(sessionId, waitlistId));
        }
    }
}
