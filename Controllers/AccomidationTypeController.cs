using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;


namespace Suvarna.DGMS.Api.Controllers
{
    [Route("api/accommodation")]
    [ApiController]
    public class AccommodationTypeController : ControllerBase
    {
        private readonly AccomidationTypeService _service;

        public AccommodationTypeController(AccomidationTypeService service)
        {
            _service = service;
        }
        [HttpPost("save")]
        public IActionResult Insupd([FromBody] AccomidationType accomidation)
        {
            return Ok(_service.inUpdService(accomidation));
        }
        //        [HttpPost("create")]
        //        public IActionResult Create([FromBody] AccomidationType accomidation)
        //        {
        //            return Ok(_service.Create(accomidation));
        //        }

        //<<<<<<< HEAD
        //        [HttpPost("update")]
        //        public IActionResult Update([FromBody] AccomidationType accomidation)
        //        {
        //            return Ok(_service.Update(accomidation));
        //        }
        //        [HttpPost("delete")]
        //        public IActionResult Delete([FromBody] deleteAccomidationType accomidation)
        //        {
        //            return Ok(_service.Delete(accomidation));
        //        }
        [HttpGet("list")]
        public IActionResult GetAccommodationTypes()
        {
            return Ok(_service.Getall());
            //=======
            //        [HttpGet("list")]
            //        public IActionResult GetAll()
            //        {
            //            return Ok(_service.Getall());
            //>>>>>>> 42f3f470f5e862c98cb3c8c8ef76e187668b4ad6
        }
    }
}