using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Toll.Reporting.Api.Repositories.Interfaces;
using Toll.Reporting.Api.DTOs;

namespace Toll.Reporting.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountUsageSummaryController : ControllerBase
    {
        private readonly IAccountUsageSummaryRepository _repository;
        private readonly ILogger<AccountUsageSummaryController> _logger;

        public AccountUsageSummaryController(
            IAccountUsageSummaryRepository repository,
            ILogger<AccountUsageSummaryController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("GetSummary")]
        public async Task<IActionResult> GetSummary(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                // 💡 Guard clause: Dates must be passed
                if (!startDate.HasValue || !endDate.HasValue)
                {
                    return BadRequest(new
                    {
                        message = "StartDate and EndDate are required.",
                        example = "/api/AccountUsageSummary/GetSummary?startDate=2025-01-01&endDate=2025-01-31"
                    });
                }

                _logger.LogInformation(
                    "📊 Fetching Account Usage Summary from {Start} to {End}",
                    startDate, endDate
                );

                // 💡 Call repository
                var result = await _repository.GetSummaryAsync(startDate.Value, endDate.Value);

                if (result == null)
                {
                    _logger.LogWarning("⚠ Repository returned NULL summary.");
                    return NotFound(new { message = "No summary data found." });
                }

                _logger.LogInformation("✅ Summary returned successfully.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Error fetching Account Usage Summary between {Start} and {End}",
                    startDate, endDate
                );

                return StatusCode(500, new
                {
                    message = "Internal Server Error while generating summary report.",
                    error = ex.Message
                });
            }
        }
    }
}
