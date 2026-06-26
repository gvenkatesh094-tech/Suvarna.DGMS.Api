using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [Route("api/Building")]
    [ApiController]
    public class BuildingController : ControllerBase
    {
        private readonly BuildingService _service;

        public BuildingController(BuildingService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] Building building)
        {
            return Ok(_service.Create(building));
        }

        [HttpPut("update")]
        public IActionResult Update([FromBody] Building building)
        {
            return Ok(_service.Update(building));
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
