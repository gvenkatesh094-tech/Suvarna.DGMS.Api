using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [ApiController]
    [Route("api/property")]
    public class PropertyController : ControllerBase
    {
        private readonly PropertyService _service;

        public PropertyController(PropertyService service)
        {
            _service = service;
        }


        [HttpPost("generatecode")]
        public IActionResult GenerateCode([FromBody] GenerateCodeRequest request)
        {
            return Ok(_service.GenerateCode(request));
        }
        [HttpPost("save")]
        //[Authorize]
        public IActionResult Insupd([FromBody] Property req)
           => Ok(_service.inUpdService(req));

        [HttpPost("create")]
        //[Authorize]
        public IActionResult Create([FromBody] Property req)
            => Ok(_service.Create(req));

        [HttpPost("update")]
        //[Authorize]
        public IActionResult update([FromBody] Property req)
    => Ok(_service.Update(req));

        [HttpPost("delete")]
        public IActionResult Delete([FromBody] Property req)
            => Ok(_service.Delete(req));

        [HttpGet("list")]
        //[Authorize]
        public IActionResult List()
               => Ok(_service.GetAll());
    }
}
