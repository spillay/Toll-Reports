using Microsoft.AspNetCore.Mvc;
using Toll.Reporting.Api.Repositories;
using Toll.Reporting.Api.Repositories.Interfaces;

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

        [HttpGet("details")]
        public async Task<IActionResult> Get(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] int? shiftId = null)
        {
            if (startDate == default || endDate == default)
            {
                return BadRequest(new
                {
                    message = "startDate and endDate are required."
                });
            }

            if (endDate < startDate)
            {
                return BadRequest(new
                {
                    message = "endDate cannot be earlier than startDate."
                });
            }

            try
            {
                var report = await _repo.GetEndOfDayAsync(startDate, endDate, shiftId);

                if (report == null)
                {
                    return NotFound(new
                    {
                        message = "No End Of Day data found for the given date range."
                    });
                }

                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to generate End Of Day Report. StartDate: {StartDate}, EndDate: {EndDate}, ShiftId: {ShiftId}",
                    startDate,
                    endDate,
                    shiftId);

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "Internal server error occurred while generating the End Of Day report."
                });
            }
        }
    }
}