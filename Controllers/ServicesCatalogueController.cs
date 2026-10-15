using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [Route("api/servicesCatalogue")]
    [Authorize]
    [ApiController]
    public class ServicesCatalogueController : ControllerBase
    {
        private readonly ServicesCatalogueService _service;

        public ServicesCatalogueController(ServicesCatalogueService service)
        {
            _service = service;
        }

        [HttpPost("save")]
        public IActionResult Insupd([FromBody] ServiceCatalogue serviceCatalogue)
        {
            return Ok(_service.inUpdService(serviceCatalogue));
        }

        [HttpGet("list")]
        public IActionResult GetServicesCatalogue()
        {
            return Ok(_service.GetAll());
        }
    }
}