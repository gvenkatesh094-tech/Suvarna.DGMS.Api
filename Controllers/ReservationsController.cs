using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
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
        [HttpPost("mark-noshow")]
        public IActionResult MarkNoShow([FromQuery] int sessionId, [FromQuery] int? graceHours = null)
        {
            return Ok(_service.MarkNoShowReservations(sessionId, graceHours));
        }
    }
}