using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Helpers;

namespace Suvarna.DGMS.Api.Controllers
{
    /// <summary>
    /// NEW: guest ID proof / photo download for logged-in users only.
    /// GET api/documents/guest?path=/secure/id/{guid}.jpg
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/documents")]
    public class DocumentsController : ControllerBase
    {
        [HttpGet("guest")]
        public IActionResult GetGuestDocument([FromQuery] string? path)
        {
            string? full = SecureDocumentStore.Resolve(path, out string contentType);
            if (full == null)
                return NotFound(new { status = false, message = "Document not found." });

            Response.Headers["Cache-Control"] = "no-store";
            Response.Headers["X-Content-Type-Options"] = "nosniff";
            return PhysicalFile(full, contentType);
        }
    }
}
