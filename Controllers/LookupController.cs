using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/lookup")]
    public class LookupController : ControllerBase
    {
        private readonly LookupService _service;

        public LookupController(LookupService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetLookup(
            [FromQuery] string lookupType)
        {
            return Ok(
                _service.GetLookup(lookupType)
            );
        }
    }
}