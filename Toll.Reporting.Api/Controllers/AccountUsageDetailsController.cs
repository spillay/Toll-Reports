using Microsoft.AspNetCore.Mvc;
using Toll.Reporting.Api.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Toll.Reporting.Api.DTOs;

namespace Toll.Reporting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountUsageDetailsController(
    IAccountUsageDetailsRepository repository,
    ILogger<AccountUsageDetailsController> logger
) : ControllerBase
{
    /// <summary>
    /// Fetch Account Usage Details + Summary between startDate and endDate.
    /// </summary>
    [HttpGet("GetDetails")]
    public async Task<IActionResult> GetDetails(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            // VALIDATION
            if (!startDate.HasValue || !endDate.HasValue)
            {
                return BadRequest(new
                {
                    message = "StartDate and EndDate are required.",
                    example = "/api/AccountUsageDetails/GetDetails?startDate=2025-01-01&endDate=2025-01-31"
                });
            }

            logger.LogInformation("📊 Fetching Account Usage Details from {Start} to {End}", startDate, endDate);

            // GET REPORT
            var result = await repository.GetAccountUsageDetailsAsync(startDate.Value, endDate.Value);

            if (result == null)
            {
                logger.LogWarning("⚠️ Repository returned NULL for the date range.");
                return NotFound(new { message = "No data returned." });
            }

            if (result.Details == null || result.Details.Count == 0)
            {
                logger.LogWarning("⚠️ No details found between {Start} and {End}", startDate, endDate);
            }

            logger.LogInformation("✅ Summary + {Count} detail rows returned.", result.Details.Count);

            // OK RESULT
            return Ok(new
            {
                summary = result.Summary,
                details = result.Details
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "❌ Error fetching Account Usage Details between {Start} and {End}",
                startDate, endDate);

            return StatusCode(500, new
            {
                message = "Internal Server Error while generating the report.",
                error = ex.Message
            });
        }
    }
}
