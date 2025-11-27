using Microsoft.AspNetCore.Mvc;
using Toll.Reporting.Api.Repositories;

namespace Toll.Reporting.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EndOfDayReportController : ControllerBase
    {
        private readonly IEndOfDayReportRepository _repo;
        private readonly ILogger<EndOfDayReportController> _logger;

        public EndOfDayReportController(IEndOfDayReportRepository repo,
                                        ILogger<EndOfDayReportController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet("Get")]
        public async Task<IActionResult> Get(DateTime reportDate)
        {
            var data = await _repo.GetEndOfDayReportAsync(reportDate);
            return Ok(data);
        }
    }

}
