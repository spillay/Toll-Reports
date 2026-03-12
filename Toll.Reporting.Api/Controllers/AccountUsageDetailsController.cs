using Microsoft.AspNetCore.Mvc;
using Toll.Reporting.Api.Repositories.Interfaces;

namespace Toll.Reporting.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountUsageDetailsController : ControllerBase
    {
        private readonly IAccountUsageDetailsRepository _repository;
        private readonly ILogger<AccountUsageDetailsController> _logger;

        public AccountUsageDetailsController(
            IAccountUsageDetailsRepository repository,
            ILogger<AccountUsageDetailsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("SearchAccounts")]
        public async Task<IActionResult> SearchAccounts([FromQuery] string q, [FromQuery] int take = 20)
        {
            try
            {
                q = (q ?? string.Empty).Trim();

                if (q.Length < 2)
                    return Ok(new List<object>());

                take = Math.Clamp(take, 1, 50);

                var results = await _repository.SearchAccountsAsync(q, take);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching accounts for Account Usage Details. Query: {Query}", q);
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }

        [HttpGet("GetDetails")]
        public async Task<IActionResult> GetDetails(
            [FromQuery] string? accountNumber,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                accountNumber = (accountNumber ?? string.Empty).Trim();

                if (string.IsNullOrWhiteSpace(accountNumber))
                    return BadRequest(new { message = "AccountNumber is required." });

                if (!startDate.HasValue || !endDate.HasValue)
                    return BadRequest(new { message = "StartDate and EndDate are required." });

                var result = await _repository.GetAccountUsageDetailsAsync(
                    accountNumber,
                    startDate.Value,
                    endDate.Value);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error fetching Account Usage Details for account {AccountNumber} between {StartDate} and {EndDate}.",
                    accountNumber,
                    startDate,
                    endDate);

                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }
    }
}