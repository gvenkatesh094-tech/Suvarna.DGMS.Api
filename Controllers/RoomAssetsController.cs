using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [ApiController]
    [Route("api/roomassets")]
    public class RoomAssetsController : ControllerBase
    {
        private readonly RoomAssetsService _service;

        public RoomAssetsController(RoomAssetsService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAssets([FromQuery] int sessionId)
        {
            return Ok(_service.GetAssets(sessionId));
        }

        [HttpGet("rooms")]
        public IActionResult GetRooms([FromQuery] int sessionId)
        {
            return Ok(_service.GetRooms(sessionId));
        }

        [HttpPost("save")]
        public IActionResult SaveAsset([FromBody] SaveRoomAssetRequest request)
        {
            return Ok(_service.SaveAsset(request));
        }

        [HttpPost("repair")]
        public IActionResult LogRepair([FromBody] LogAssetRepairRequest request)
        {
            return Ok(_service.LogRepair(request));
        }

        [HttpGet("due-for-pm")]
        public IActionResult GetAssetsDueForPm([FromQuery] int sessionId)
        {
            return Ok(_service.GetAssetsDueForPm(sessionId));
        }
    }
}
