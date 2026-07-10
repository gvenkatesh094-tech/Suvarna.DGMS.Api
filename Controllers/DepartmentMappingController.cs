using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [Route("api/deptMap")]
    [ApiController]
    public class DepartmentMappingController : ControllerBase
    {
        private readonly DepartmentMappingService _Service;

        public DepartmentMappingController(DepartmentMappingService Service)
        {
            _Service = Service;
        }

        [HttpGet("list")]
        public IActionResult Get()
        {
            return Ok(_Service.GetAll());
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] DepartmentCostMapping model)
        {
            return Ok(_Service.Create(model));
        }

        [HttpPost("update")]
        public IActionResult Update([FromBody] DepartmentCostMapping model)
        {

            return Ok(_Service.Update(model));
        }

    }
}
