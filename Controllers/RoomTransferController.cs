using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/roomtransfer")]
    public class RoomTransferController : ControllerBase
    {
        private readonly RoomTransferService _service;

        public RoomTransferController(RoomTransferService service)
        {
            _service = service;
        }

        // ==========================================
        // "Select Booking" dropdown
        // ==========================================

        [HttpGet("bookings")]
        public IActionResult GetBookings([FromQuery] int sessionId, [FromQuery] int? propertyId = null)
        {
            return Ok(_service.GetBookings(sessionId, propertyId));
        }

        // ==========================================
        // "Transfer To Room" dropdown
        // ==========================================

        [HttpGet("available-rooms")]
        public IActionResult GetAvailableRooms(
            [FromQuery] int sessionId,
            [FromQuery] int? propertyId = null,
            [FromQuery] int? excludeRoomId = null)
        {
            return Ok(_service.GetAvailableRooms(sessionId, propertyId, excludeRoomId));
        }

        // ==========================================
        // "Confirm Transfer" button
        // ==========================================

        [HttpPost("confirm")]
        public IActionResult ConfirmTransfer([FromBody] ConfirmTransferRequest request)
        {
            return Ok(_service.ConfirmTransfer(request));
        }
    }
}
