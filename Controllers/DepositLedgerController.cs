using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/depositledger")]
    public class DepositLedgerController : ControllerBase
    {
        private readonly DepositLedgerService _service;

        public DepositLedgerController(DepositLedgerService service)
        {
            _service = service;
        }

        // ==========================================
        // GET DEPOSIT LEDGER
        // GET api/depositledger?sessionId=1&propertyId=&status=
        // ==========================================

        [HttpGet]
        public IActionResult GetDepositLedger(
            [FromQuery] int sessionId,
            [FromQuery] int? propertyId,
            [FromQuery] string? status = null)
        {
            if (sessionId <= 0)
            {
                return BadRequest("A valid session is required.");
            }

            return Ok(_service.GetDepositLedger(sessionId, propertyId, status));
        }

        // ==========================================
        // GET DEPOSIT TRANSACTION HISTORY
        // GET api/depositledger/history?sessionId=1&bookingId=101
        // ==========================================

        [HttpGet("history")]
        public IActionResult GetDepositTxnHistory(
            [FromQuery] int sessionId,
            [FromQuery] int bookingId)
        {
            if (sessionId <= 0)
            {
                return BadRequest("A valid session is required.");
            }

            if (bookingId <= 0)
            {
                return BadRequest("A valid booking id is required.");
            }

            return Ok(_service.GetDepositTxnHistory(sessionId, bookingId));
        }

        // ==========================================
        // REFUND DEPOSIT
        // POST api/depositledger/refund
        // ==========================================

        [HttpPost("refund")]
        public IActionResult RefundDeposit([FromBody] DepositRefundRequest request)
        {
            if (request == null)
            {
                return BadRequest("Refund request payload is required.");
            }

            if (request.sessionId <= 0)
            {
                return BadRequest("A valid session is required.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(_service.RefundDeposit(request));
        }
    }
}
