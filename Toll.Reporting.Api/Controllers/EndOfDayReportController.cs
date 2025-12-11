using Microsoft.AspNetCore.Mvc;
using Toll.Reporting.Api.Repositories;
using Toll.Reporting.Api.DTOs.EndOfDay;

namespace Toll.Reporting.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EndOfDayReportController : ControllerBase
    {
        private readonly IEndOfDayReportRepository _repo;
        private readonly ILogger<EndOfDayReportController> _logger;

        public EndOfDayReportController(
            IEndOfDayReportRepository repo,
            ILogger<EndOfDayReportController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate == default || endDate == default)
                return BadRequest("StartDate and EndDate are required.");

            if (endDate < startDate)
                return BadRequest("EndDate cannot be earlier than StartDate.");

            try
            {
                var report = await _repo.GetEndOfDayAsync(startDate, endDate);

                if (report == null)
                    return NotFound("No End Of Day data found for the given date range.");

                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate End Of Day Report.");
                return StatusCode(500, "Internal server error occurred.");
            }
        }
    }
}
