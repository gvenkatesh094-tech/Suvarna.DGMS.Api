using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Controllers
{
    [Route("api/deptMap")]
    [ApiController]
    public class DepartmentMappingController : ControllerBase
    {
        private readonly IDepartmentMappingRepository _repo;

        public DepartmentMappingController(IDepartmentMappingRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_repo.GetAll());
        }

        [HttpPost]
        public IActionResult Save([FromBody] DepartmentCostMapping model)
        {
            var result = _repo.Save(model);

            return Ok(new
            {
                success = result.Success,
                message = result.Message,
                id = result.NewId
            });
        }

        [HttpPut]
        public IActionResult Update([FromBody] DepartmentCostMapping model)
        {
            var result = _repo.Update(model);

            return Ok(new
            {
                success = result.Success,
                message = result.Message
            });
        }

        [HttpDelete("{id}/{userId}")]
        public IActionResult Delete(int id, int userId)
        {
            var result = _repo.Delete(id, userId);

            return Ok(new
            {
                success = result.Success,
                message = result.Message
            });
        }
    }
}
