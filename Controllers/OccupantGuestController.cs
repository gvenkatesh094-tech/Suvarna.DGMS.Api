using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [Route("api/occupantGuest")]
    [Authorize]
    [ApiController]
    public class OccupantGuestController : ControllerBase
    {
        private readonly OccupantGuestService _service;

        public OccupantGuestController(OccupantGuestService service)
        {
            _service = service;
        }


        [HttpPost("save")]
        public IActionResult Insupd([FromBody] OccupantGuest Occupant)
        {
            return Ok(_service.inUpdService(Occupant));
        }
        
        [HttpGet("list")]
        public IActionResult GetAccommodationTypes()
        {
            return Ok(_service.GetAll());
        }

    }
}
