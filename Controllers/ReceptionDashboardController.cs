using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [ApiController]
    [Route("api/reception-dashboard")]
    public class ReceptionDashboardController : ControllerBase
    {
        private readonly ReceptionDashboardService _service;

        public ReceptionDashboardController(
            ReceptionDashboardService service)
        {
            _service = service;
        }

        //------------------------------------------------------
        // Dashboard Summary
        //------------------------------------------------------

        [HttpGet("summary")]
        public IActionResult GetSummary()
        {
            return Ok(_service.GetDashboardSummary());
        }

        //------------------------------------------------------
        // Active Bookings
        //------------------------------------------------------

        [HttpGet("active-bookings")]
        public IActionResult GetActiveBookings()
        {
            return Ok(_service.GetActiveBookings());
        }

        //------------------------------------------------------
        // Room Status
        //------------------------------------------------------

        [HttpGet("room-status")]
        public IActionResult GetRoomStatus()
        {
            return Ok(_service.GetRoomStatus());
        }

        //------------------------------------------------------
        // Alerts
        //------------------------------------------------------

        [HttpGet("alerts")]
        public IActionResult GetAlerts()
        {
            return Ok(_service.GetAlerts());
        }

        //------------------------------------------------------
        // Revenue
        //------------------------------------------------------

        [HttpGet("revenue")]
        public IActionResult GetRevenue()
        {
            return Ok(_service.GetRevenue());
        }
    }
}