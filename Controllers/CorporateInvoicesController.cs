using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [ApiController]
    [Route("api/corporateinvoices")]
    public class CorporateInvoicesController : ControllerBase
    {
        private readonly CorporateInvoicesService _service;

        public CorporateInvoicesController(CorporateInvoicesService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetInvoices([FromQuery] int sessionId)
        {
            return Ok(_service.GetInvoices(sessionId));
        }

        [HttpGet("agreements")]
        public IActionResult GetAgreements([FromQuery] int sessionId)
        {
            return Ok(_service.GetAgreements(sessionId));
        }

        [HttpPost("generate")]
        public IActionResult GenerateInvoice([FromBody] GenerateCorpInvoiceRequest request)
        {
            return Ok(_service.GenerateInvoice(request));
        }

        [HttpGet("{invoiceId:int}")]
        public IActionResult GetInvoiceDetail(int invoiceId, [FromQuery] int sessionId)
        {
            return Ok(_service.GetInvoiceDetail(sessionId, invoiceId));
        }
    }
}
