using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [Route("api/roomAmenities")]
    [ApiController]
    public class RoomAmenitiesController : ControllerBase
    {
        private readonly RoomAmenitiesService _service;

        public RoomAmenitiesController(
            RoomAmenitiesService service)
        {
            _service = service;
        }

        [HttpPost("save")]
        public IActionResult InsUpd(
            [FromBody] RoomAmenity amenity)
        {
            return Ok(
                _service.InUpdRoomAmenity(
                    amenity));
        }

        [HttpGet("list")]
        public IActionResult GetRoomAmenities()
        {
            return Ok(
                _service.GetAll());
        }
    }
}