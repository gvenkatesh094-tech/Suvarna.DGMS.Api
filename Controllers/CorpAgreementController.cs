using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [Route("api/Agreement")]
    [ApiController]
    public class CorpAgreementController : ControllerBase
    {
        private readonly CorpAgreementService _service;

        public CorpAgreementController(CorpAgreementService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] CorpAgreement _Corp)
        {
            return Ok(_service.Create(_Corp));
        }

        [HttpPost("update")]
        public IActionResult Update([FromBody] CorpAgreement _Corp)
        {
            return Ok(_service.Update(_Corp));
        }

        [HttpGet("list")]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAll());
        }


    }
}
