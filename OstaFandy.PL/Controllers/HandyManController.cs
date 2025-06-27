using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OstaFandy.PL.BL;
using OstaFandy.PL.BL.IBL;

namespace OstaFandy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HandyManController : ControllerBase
    {
        private readonly IHandyManService _HandyManService;
        public HandyManController(IHandyManService HandyManService)
        {
            _HandyManService = HandyManService;
        }

        [HttpGet("GetHandyManStats/{id}")]
        public IActionResult GetHandyManStats(int id)
        {
            var todayJobsCount = _HandyManService.GetHandyManStats(id);
            return Ok(todayJobsCount);
        }

    }
}
