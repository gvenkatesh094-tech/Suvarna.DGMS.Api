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

        [HttpPost("create")]
        public IActionResult Create([FromBody] Floor floor)
        {
            return Ok(_service.Create(floor));
        }

        [HttpPut("update")]
        public IActionResult Update([FromBody] Floor floor)
        {
            return Ok(_service.Update(floor));
        }
        [HttpPost("delete")]
        public IActionResult Delete([FromBody] DeleteFloorRequest building)
        {
            return Ok(_service.Delete(building));
        }

        [HttpGet("list")]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            return Ok(_service.GetById(id));
        }
    }
}