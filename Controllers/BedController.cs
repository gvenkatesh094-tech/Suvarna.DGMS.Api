using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/bed")]
    public class BedController : ControllerBase
    {
        private readonly BedService _service;

        public BedController(BedService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] Bed bed)
        {
            return Ok(_service.Create(bed));
        }

        [HttpPut("update")]
        public IActionResult Update([FromBody] Bed bed)
        {
            return Ok(_service.Update(bed));
        }

        [HttpGet("list")]
        public IActionResult GetAll(int sessionId, int locationId)
        {
            return Ok(_service.GetAll(sessionId, locationId));
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            return Ok(_service.GetById(id));
        }

        // REMOVED: PUT api/bed/status - it called DGMS.PR_DGMS_UPDATE_BED_STATUS, which does not exist
        // in the database, and no screen used it. Bed status is set by check-in / checkout / housekeeping.
 
    }
}