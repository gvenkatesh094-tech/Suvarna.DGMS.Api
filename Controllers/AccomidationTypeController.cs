using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;


namespace Suvarna.DGMS.Api.Controllers
{
    [Route("api/accommodation")]
    [Authorize]
    [ApiController]
    public class AccommodationTypeController : ControllerBase
    {
        private readonly AccomidationTypeService _service;

        public AccommodationTypeController(AccomidationTypeService service)
        {
            _service = service;
        }
        [HttpPost("save")]
        public IActionResult Insupd([FromBody] AccomidationType accomidation)
        {
            return Ok(_service.inUpdService(accomidation));
        }
        
        [HttpGet("list")]
        public IActionResult GetAccommodationTypes(int sessionId, int locationId)
        {
            return Ok(_service.Getall( sessionId,  locationId));
           
        }
        [HttpGet("categorylist")]
        public IActionResult GetcaterigyAccommodationTypes(int sessionId, int locationId)
        {
            return Ok(_service.Getcaterigyall(sessionId, locationId));

        }
    }
}