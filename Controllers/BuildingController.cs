using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [Route("api/Building")]
    [Authorize]
    [ApiController]
    public class BuildingController : ControllerBase
    {
        private readonly BuildingService _service;

        public BuildingController(BuildingService service)
        {
            _service = service;
        }
        [HttpPost("save")]
        public IActionResult Insupd([FromBody] Building req)
           => Ok(_service.inUpdService(req));

        

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
