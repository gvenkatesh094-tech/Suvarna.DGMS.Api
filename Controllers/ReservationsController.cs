using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/reservations")]
    public class ReservationsController
        : ControllerBase
    {
        private readonly ReservationsService _service;

        public ReservationsController(
            ReservationsService service)
        {
            _service = service;
        }


        // ==========================================
        // CREATE / UPDATE RESERVATION
        // ==========================================

        [HttpPost("save")]
        public IActionResult Save([FromBody] Reservation request)
        {
            if (request == null)
            {
                return BadRequest(
                    new
                    {
                        message = "Invalid reservation request."
                    }
                );
            }

            return Ok(
                _service.SaveReservation(
                    request
                )
            );
        }
        // ==========================================
        // GET RESERVATIONS FOR GRID
        // ==========================================

        [HttpGet("list")]
        public IActionResult GetReservations([FromQuery] int sessionId)
        {
            return Ok(_service.GetReservations(sessionId));
        }
        [HttpGet("availability")]
        public IActionResult CheckAvailability([FromQuery] int sessionId, [FromQuery] int propertyId, [FromQuery] int accmTypeId, [FromQuery] DateTime checkinDt, [FromQuery] DateTime checkoutDt)
        {
            if (checkoutDt <= checkinDt)
            {
                return BadRequest(
                    new
                    {
                        message = "Check-out date must be greater than check-in date."
                    }
                );
            }
            return Ok(_service.CheckAvailability(sessionId, propertyId, accmTypeId, checkinDt, checkoutDt)
            );
        }
        // NEW: POST api/reservations/cancel
        [HttpPost("cancel")]
        public IActionResult Cancel([FromBody] ReservationCancelRequest request)
        {
            if (request == null)
                return BadRequest(new { message = "Invalid cancel request." });

            return Ok(_service.CancelReservation(request));
        }

        // 03-Nov: GET api/reservations/guest-check - last stay of the mobile (to fill the form)
        // and its reservations / stays that overlap the dates (duplicate warning)
        [HttpGet("guest-check")]
        public IActionResult GuestCheck([FromQuery] int sessionId, [FromQuery] string mobileNo, [FromQuery] DateTime? checkinDt, [FromQuery] DateTime? checkoutDt, [FromQuery] int bookingId = 0)
        {
            return Ok(_service.GetGuestCheck(sessionId, mobileNo, checkinDt, checkoutDt, bookingId));
        }

        [HttpPost("mark-noshow")]
        public IActionResult MarkNoShow([FromQuery] int sessionId, [FromQuery] int? graceHours = null)
        {
            return Ok(_service.MarkNoShowReservations(sessionId, graceHours));
        }
    }
}