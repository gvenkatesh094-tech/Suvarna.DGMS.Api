using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [ApiController]
    [Route("api/room")]
    public class RoomController : ControllerBase
    {
        private readonly RoomService _service;

        public RoomController(RoomService service)
        {
            _service = service;
        }


        [HttpPost("create")]
        public IActionResult Create([FromBody] Room room)
        {
            return Ok(_service.Create(room));
        }

        [HttpPut("update")]
        public IActionResult Update([FromBody] Room room)
        {
            return Ok(_service.Update(room));
        }

        [HttpGet("list")]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAll());
        }

        //[HttpGet("{id}")]
        //public IActionResult GetById(int id)
        //{
        //    return Ok(_service.GetById(id));
        //}
    }
}