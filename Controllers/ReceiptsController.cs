using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    /// <summary>
    /// NEW 04-Nov: standard receipts. Every screen calls this once per print / reprint.
    /// sessionId in the body is replaced with the checked X-Session-Id by SessionBindingFilter.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/receipts")]
    public class ReceiptsController : ControllerBase
    {
        private readonly ReceiptsService _service;

        public ReceiptsController(ReceiptsService service)
        {
            _service = service;
        }

        // POST api/receipts/print { sessionId, docType, docNo, sizeGroup, propertyId?, reprint }
        [HttpPost("print")]
        public IActionResult Print([FromBody] ReceiptPrintRequest req)
        {
            if (req == null || req.SessionId <= 0) return BadRequest("A valid session is required.");
            return Ok(_service.LogPrint(req));
        }
    }
}
