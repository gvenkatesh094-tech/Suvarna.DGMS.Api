using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [Route("api/gstconfig")]
    [Authorize]
    [ApiController]
    public class GstConfigController : ControllerBase
    {
        private readonly GstConfigService _service;

        public GstConfigController(GstConfigService service)
        {
            _service = service;
        }

        #region SAVE
        [HttpPost("save")]
        public IActionResult Save([FromBody] GstConfig gst)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _service.Save(gst);
            return result.Status ? Ok(result) : BadRequest(result);
        }
        #endregion

        #region UPDATE
        [HttpPut("update")]
        public IActionResult Update([FromBody] GstConfig gst)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _service.Update(gst);
            return result.Status ? Ok(result) : BadRequest(result);
        }
        #endregion

        #region GET ALL
        [HttpGet("list")]
        public IActionResult GetAll()
        {
            var result = _service.GetAll();
            return result.Status ? Ok(result) : StatusCode(500, result);
        }
        #endregion

        #region GET BY ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = _service.GetById(id);
            if (!result.Status && result.Code == "404") return NotFound(result);
            return result.Status ? Ok(result) : StatusCode(500, result);
        }
        #endregion
    }
}