using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    /// <summary>
    /// NEW: Pine Labs POS machine payments through PayLink. The browser never talks to PayLink;
    /// it calls these APIs and the API calls PayLink with the credentials from appsettings.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/pos-payment")]
    public class PosPaymentController : ControllerBase
    {
        private readonly PosPaymentService _service;

        public PosPaymentController(PosPaymentService service)
        {
            _service = service;
        }

        // GET api/pos-payment/settings  -> which modes go to the machine, manual RRN allowed or not
        [HttpGet("settings")]
        public IActionResult GetSettings()
        {
            return Ok(_service.GetSettings());
        }

        // GET api/pos-payment/devices?sessionId=
        [HttpGet("devices")]
        public async Task<IActionResult> GetDevices([FromQuery] int sessionId)
        {
            return Ok(await _service.GetDevicesAsync(sessionId));
        }

        // POST api/pos-payment/initiate  -> sends the amount to the machine
        [HttpPost("initiate")]
        public async Task<IActionResult> Initiate([FromBody] PosInitiateRequest request)
        {
            return Ok(await _service.InitiateAsync(request));
        }

        // POST api/pos-payment/status
        [HttpPost("status")]
        public async Task<IActionResult> CheckStatus([FromBody] PosStatusRequest request)
        {
            return Ok(await _service.CheckStatusAsync(request));
        }

        // POST api/pos-payment/close  -> frees the machine when the payment never completed
        [HttpPost("close")]
        public async Task<IActionResult> Close([FromBody] PosCloseRequest request)
        {
            return Ok(await _service.CloseAsync(request));
        }

        // GET api/pos-payment/open?sessionId=&purpose=CHECKOUT&refId=123&refNo=
        [HttpGet("open")]
        public async Task<IActionResult> GetOpen([FromQuery] int sessionId, [FromQuery] string purpose, [FromQuery] int refId = 0, [FromQuery] string? refNo = null)
        {
            return Ok(await _service.GetOpenAsync(sessionId, purpose, refId, refNo));
        }

        // GET api/pos-payment/list?sessionId=&fromDt=&toDt=&filter=ALL|OPEN|UNSAVED|MISMATCH|FAILED
        [HttpGet("list")]
        public async Task<IActionResult> List([FromQuery] int sessionId, [FromQuery] DateTime fromDt, [FromQuery] DateTime toDt, [FromQuery] string? filter = null)
        {
            return Ok(await _service.ListAsync(sessionId, fromDt, toDt, filter));
        }

        // GET api/pos-payment/{posTxnId}?sessionId=
        [HttpGet("{posTxnId:long}")]
        public async Task<IActionResult> Get(long posTxnId, [FromQuery] int sessionId)
        {
            return Ok(await _service.GetAsync(sessionId, posTxnId));
        }

        // GET api/pos-payment/{posTxnId}/logs?sessionId=   (masked PayLink requests / responses)
        [HttpGet("{posTxnId:long}/logs")]
        public async Task<IActionResult> GetLogs(long posTxnId, [FromQuery] int sessionId)
        {
            return Ok(await _service.GetLogsAsync(sessionId, posTxnId));
        }
    }
}
