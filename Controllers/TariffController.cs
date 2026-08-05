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



        [HttpPost("save")]
        public IActionResult Save(            [FromBody] TariffSaveRequest req)
        {
            return Ok(
                _service.SaveTariff(req)
            );
        }

        [HttpGet("list")]
        public IActionResult List(int sessionId, int locationId)
        {
            return Ok(
                _service.GetAll(sessionId, locationId)
            );
        }
        [HttpGet("billingmodellist")]
        public IActionResult BillingmodelList(int sessionId, int locationId)
        {
            return Ok(
                _service.GetBillingmodelAll(sessionId, locationId)
            );
        }
    }
}