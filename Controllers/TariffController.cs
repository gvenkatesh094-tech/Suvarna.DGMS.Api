using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [ApiController]
    [Route("api/tariff")]
    public class TariffController : ControllerBase
    {
        private readonly TariffService _service;

        public TariffController(
            TariffService service)
        {
            _service = service;
        }

        [HttpPost("generatecode")]
        public IActionResult GenerateCode(
            [FromBody] GenerateCodeRequest request)
        {
            return Ok(
                _service.GenerateCode(request)
            );
        }

        [HttpPost("save")]
        public IActionResult Save(
            [FromBody] Tariff req)
        {
            return Ok(
                _service.SaveTariff(req)
            );
        }

        [HttpGet("list")]
        public IActionResult List()
        {
            return Ok(
                _service.GetAll()
            );
        }
    }
}