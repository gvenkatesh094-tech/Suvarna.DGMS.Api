using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [ApiController]
    [Route("api/bed")]
    public class BedController : ControllerBase
    {
        private readonly BedService _service;

        public BedController(BedService service)
        {
            _service = service;
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

        [HttpPut("status")]
        public IActionResult UpdateStatus([FromBody] Bed bed)
        {
            return Ok(_service.UpdateStatus(bed));
        }
    }
}