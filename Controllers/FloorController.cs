using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [ApiController]
    [Route("api/floor")]
    public class FloorController : ControllerBase
    {
        private readonly FloorService _service;

        public FloorController(FloorService service)
        {
            _service = service;
        }

        [HttpPost("save")]
        public IActionResult Insupd([FromBody] Floor floor)
        {
            return Ok(_service.inUpdService(floor));
        }       
        [HttpGet("list")]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAll());
        }
       
    }
}