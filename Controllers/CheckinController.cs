using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;
using System.Text.Json;

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
        public IActionResult GetRooms([FromQuery] int sessionId, [FromQuery] int? locationId , [FromQuery] int? occCatId , [FromQuery] DateTime checkinDt , [FromQuery] DateTime checkOutDt, [FromQuery] int? bookinId, int? accmTypeId = null)
        {
            return Ok(_service.GetRooms(sessionId, locationId, occCatId,  checkinDt,  checkOutDt,   bookinId, accmTypeId ));
        }

        // ==========================================
        // TARIFF LOOKUP
        // ==========================================

        [HttpGet("tariff")]
        public IActionResult GetTariff([FromQuery] int sessionId, [FromQuery] int propertyId, [FromQuery] int accmTypeId, [FromQuery] int occCatId, [FromQuery] DateTime checkinDt, [FromQuery] string? billingModel = null)
        {
            return Ok(_service.GetTariff(sessionId, propertyId, accmTypeId, occCatId, checkinDt, billingModel));
        }

        // ==========================================
        // CONFIRM CHECK-IN
        // ==========================================

        // ==========================================
        // CONFIRM CHECK-IN
        // ==========================================

        [HttpPost("confirm")]
        public IActionResult Confirm([FromForm] string request, IFormFile? idFile, IFormFile? photoFile)
        {
            var model = JsonSerializer.Deserialize<Checkin>(request);

            if (model == null)
            {
                throw new Exception("Invalid Check-In request.");
            }

            return Ok(_service.ConfirmCheckin(model, idFile, photoFile)
            );
        }
        // ==========================================
        // PATIENT LOOKUP
        // GET api/checkin/patients?searchBy=ALL|NAME&searchText=...
        // ==========================================

        [HttpGet("patients")]
        public IActionResult SearchPatients([FromQuery] string? searchBy, [FromQuery] string? searchText)
        {
            return Ok(_service.SearchPatients(searchBy, searchText));
        }
    }
}