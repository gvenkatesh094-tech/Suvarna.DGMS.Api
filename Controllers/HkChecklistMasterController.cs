using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    // 28-Oct: HK Checklist Master screen (/hk-checklist-master)
    [Authorize]
    [ApiController]
    [Route("api/hk-checklist")]
    public class HkChecklistMasterController : ControllerBase
    {
        private readonly HkChecklistMasterService _service;

        public HkChecklistMasterController(HkChecklistMasterService service)
        {
            _service = service;
        }

        // room types + properties for the template form
        [HttpGet("lookups")]
        public IActionResult GetLookups([FromQuery] int sessionId = 0)
        {
            return Ok(_service.GetLookups(sessionId));
        }

        // ==========================================
        // ITEM LIBRARY
        // ==========================================

        [HttpGet("items")]
        public IActionResult GetItems([FromQuery] int sessionId = 0)
        {
            return Ok(_service.GetItems(sessionId));
        }

        [HttpPost("items/save")]
        public IActionResult SaveItem([FromBody] HkChkItemSaveRequest request)
        {
            return Ok(_service.SaveItem(request));
        }

        // ==========================================
        // TEMPLATES (room type mapping)
        // ==========================================

        [HttpGet("templates")]
        public IActionResult GetTemplates([FromQuery] int sessionId = 0)
        {
            return Ok(_service.GetTemplates(sessionId));
        }

        [HttpGet("templates/{templateId:int}/items")]
        public IActionResult GetTemplateItems(int templateId, [FromQuery] int sessionId = 0)
        {
            return Ok(_service.GetTemplateItems(sessionId, templateId));
        }

        [HttpPost("templates/save")]
        public IActionResult SaveTemplate([FromBody] HkChkTemplateSaveRequest request)
        {
            return Ok(_service.SaveTemplate(request));
        }
    }
}
