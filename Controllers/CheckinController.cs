using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [ApiController]
    [Route("api/checkin")]
    public class CheckinController : ControllerBase
    {
        private readonly CheckinService _service;

        public CheckinController(
            CheckinService service)
        {
            _service = service;
        }

        // ==========================================
        // AVAILABLE ROOM LOOKUP
        // ==========================================

        [HttpGet("rooms")]
        public IActionResult GetRooms(
            [FromQuery] string sessionId,
            [FromQuery] int? propertyId = null,
            [FromQuery] int? buildingId = null,
            [FromQuery] int? floorId = null,
            [FromQuery] int? accmTypeId = null,
            [FromQuery] string? roomStatus = "AVAILABLE",
            [FromQuery] string? isAc = null,
            [FromQuery] string? isAccessible = null)
        {
            return Ok(
                _service.GetRooms(
                    sessionId,
                    propertyId,
                    buildingId,
                    floorId,
                    accmTypeId,
                    roomStatus,
                    isAc,
                    isAccessible
                )
            );
        }

        // ==========================================
        // TARIFF LOOKUP
        // ==========================================

        [HttpGet("tariff")]
        public IActionResult GetTariff(
            [FromQuery] string sessionId,
            [FromQuery] int propertyId,
            [FromQuery] int accmTypeId,
            [FromQuery] int occCatId,
            [FromQuery] DateTime checkinDt,
            [FromQuery] string? billingModel = null)
        {
            return Ok(
                _service.GetTariff(
                    sessionId,
                    propertyId,
                    accmTypeId,
                    occCatId,
                    checkinDt,
                    billingModel
                )
            );
        }

        // ==========================================
        // CONFIRM CHECK-IN
        // ==========================================

        [HttpPost("confirm")]
        public IActionResult Confirm([FromBody] Checkin request)
        {
            return Ok(
                _service.ConfirmCheckin(request)
            );
        }
    }
}