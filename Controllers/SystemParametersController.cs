using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [Route("api/SystemParameters")]
    [ApiController]
    public class SystemParametersController : ControllerBase
    {
        private readonly SystemParametersService _service;

        public SystemParametersController(SystemParametersService service)
        {
            _service = service;
        }

        [HttpPost("save")]
        public IActionResult InsUpd([FromBody] SystemParameter parameter)
        {
            return Ok(_service.InUpdSystemParameter(parameter));
        }

        [HttpGet("list")]
        public IActionResult GetSystemParameters()
        {
            return Ok(_service.GetAll());
        }

        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            return Ok(_service.GetCategories());
        }
    }
}