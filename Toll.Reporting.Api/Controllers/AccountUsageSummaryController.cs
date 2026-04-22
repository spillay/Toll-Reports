using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Toll.Reporting.Api.Repositories.Interfaces;

namespace Toll.Reporting.Api.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> GetSummary(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? accountNumber = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (!startDate.HasValue || !endDate.HasValue)
                {
                    return BadRequest(new
                    {
                        message = "startDate and endDate are required.",
                        example = "/api/AccountUsageSummary/GetSummary?startDate=2026-02-01&endDate=2026-03-30&page=1&pageSize=20",
                        singleAccountExample = "/api/AccountUsageSummary/GetSummary?startDate=2026-02-01&endDate=2026-03-30&accountNumber=116407&page=1&pageSize=20"
                    });
                }

                accountNumber = (accountNumber ?? string.Empty).Trim();

                if (page < 1)
                    page = 1;

                if (pageSize < 1)
                    pageSize = 20;

                _logger.LogInformation(
                    "Fetching Account Usage Summary from {Start} to {End}. AccountNumber: {AccountNumber}, Page: {Page}, PageSize: {PageSize}",
                    startDate.Value,
                    endDate.Value,
                    string.IsNullOrWhiteSpace(accountNumber) ? "ALL" : accountNumber,
                    page,
                    pageSize
                );

                var result = await _repository.GetSummaryAsync(
                    startDate.Value,
                    endDate.Value,
                    string.IsNullOrWhiteSpace(accountNumber) ? null : accountNumber,
                    page,
                    pageSize
                );

                if (result == null)
                {
                    _logger.LogWarning(
                        "Repository returned NULL summary for Start: {Start}, End: {End}, AccountNumber: {AccountNumber}, Page: {Page}, PageSize: {PageSize}",
                        startDate.Value,
                        endDate.Value,
                        string.IsNullOrWhiteSpace(accountNumber) ? "ALL" : accountNumber,
                        page,
                        pageSize
                    );

                    return NotFound(new { message = "No summary data found." });
                }

                _logger.LogInformation(
                    "Account Usage Summary returned successfully for Start: {Start}, End: {End}, AccountNumber: {AccountNumber}, Page: {Page}, PageSize: {PageSize}",
                    startDate.Value,
                    endDate.Value,
                    string.IsNullOrWhiteSpace(accountNumber) ? "ALL" : accountNumber,
                    page,
                    pageSize
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching Account Usage Summary between {Start} and {End}. AccountNumber: {AccountNumber}, Page: {Page}, PageSize: {PageSize}",
                    startDate,
                    endDate,
                    string.IsNullOrWhiteSpace(accountNumber) ? "ALL" : accountNumber,
                    page,
                    pageSize
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