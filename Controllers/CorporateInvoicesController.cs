using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [Authorize]
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

        // includeExpired=true (NEW, Round B): an agreement that ended can still be invoiced for its last period
        [HttpGet("agreements")]
        public IActionResult GetAgreements([FromQuery] int sessionId, [FromQuery] bool includeExpired = false)
        {
            return Ok(_service.GetAgreements(sessionId, includeExpired));
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

        // NEW (Round B): company payment received against the invoice
        [HttpPost("{invoiceId:int}/payment")]
        public IActionResult RecordPayment(int invoiceId, [FromBody] CorpInvoicePaymentRequest request)
        {
            request.invoiceId = invoiceId;
            return Ok(_service.RecordPayment(request));
        }

        // NEW (Round B): cancel an unpaid invoice (its stays can be invoiced again)
        [HttpPost("{invoiceId:int}/cancel")]
        public IActionResult CancelInvoice(int invoiceId, [FromBody] CorpInvoiceCancelRequest request)
        {
            request.invoiceId = invoiceId;
            return Ok(_service.CancelInvoice(request));
        }
    }
}
