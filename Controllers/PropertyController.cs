using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [Authorize]
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
        public IActionResult Insupd([FromBody] Property req)
           => Ok(_service.inUpdService(req));
        

        [HttpGet("list")]
        public IActionResult List(int sessionId, int locationId)
               => Ok(_service.GetAll( sessionId,  locationId));
    }
}
