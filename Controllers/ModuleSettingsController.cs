using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Suvarna.DGMS.Api.Models;

namespace Suvarna.DGMS.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/ModuleSettings")]
    public class ModuleSettingsController : Controller
    {
        private readonly ModuleSettings _moduleSettings;

        public ModuleSettingsController(IOptions<ModuleSettings> moduleSettings)
        {
            _moduleSettings = moduleSettings.Value;
        }

        [HttpGet("module-config")]
        public IActionResult GetModuleConfig()
        {
            return Ok(new
            {
                ParentModuleId = _moduleSettings.ParentModuleId
            });
        }
    }
}
