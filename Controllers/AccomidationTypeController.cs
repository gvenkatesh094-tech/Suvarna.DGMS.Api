using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [Route("api/accommodation")]
    [ApiController]
    public class AccommodationTypeController : ControllerBase
    {
        private readonly AccomidationTypeService _service;

        public AccommodationTypeController(AccomidationTypeService service)
        {
            _service = service;
        }

        [HttpGet("list")]
        public IActionResult GetAll()
        {
            return Ok(_service.Getall());
        }
    }
}