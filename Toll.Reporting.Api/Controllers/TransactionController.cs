using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Toll.Reporting.Api.Repositories;

namespace Toll.Reporting.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ITransactionRepository transactionRepository, ILogger<TransactionController> logger)
        {
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        /// <summary>
        /// Get Transaction Details Report with optional filters.
        /// Accepts a large set of common date formats (ISO, yyyy-MM-ddTHH:mm[:ss], yyyy-MM-dd, yyyy/MM/dd, dd/MM/yyyy).
        /// If the end date is supplied without a time part, the method treats the end as inclusive end-of-day.
        /// </summary>
        [HttpGet("details")]
        public async Task<IActionResult> GetTransactionDetails(
            [FromQuery] string? startDate,
            [FromQuery] string? endDate,
            [FromQuery] List<string>? operationalShift = null,
            [FromQuery] List<string>? tollOperators = null,
            [FromQuery] List<string>? laneNames = null,
            [FromQuery] List<string>? paymentMethods = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(startDate) || string.IsNullOrWhiteSpace(endDate))
                {
                    return BadRequest("StartDate and EndDate are required. Accepts formats: ISO 8601, yyyy-MM-ddTHH:mm[:ss], yyyy-MM-dd, yyyy/MM/dd, dd/MM/yyyy.");
                }

                // Supported formats - keep these in sync with what the client sends.
                var formats = new[]
                {
                    "o",           // ISO 8601 round-trip
                    "s",           // Sortable: yyyy-MM-ddTHH:mm:ss
                    "yyyy-MM-ddTHH:mm:ss",
                    "yyyy-MM-ddTHH:mm",
                    "yyyy-MM-dd",
                    "yyyy/MM/dd",
                    "dd/MM/yyyy",
                    "dd/MM/yyyy HH:mm:ss"
                };

                // Try strict parse first, otherwise fall back to TryParse with invariant culture
                if (!DateTime.TryParseExact(startDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedStart)
                    && !DateTime.TryParse(startDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedStart))
                {
                    return BadRequest($"Unable to parse StartDate '{startDate}'. Supported formats: yyyy-MM-ddTHH:mm[:ss], yyyy-MM-dd, yyyy/MM/dd, dd/MM/yyyy, ISO 8601.");
                }

                if (!DateTime.TryParseExact(endDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedEnd)
                    && !DateTime.TryParse(endDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedEnd))
                {
                    return BadRequest($"Unable to parse EndDate '{endDate}'.");
                }

                // If user sent a plain date (no 'T' or ':'), treat the end as inclusive end-of-day
                bool endHasTime = endDate.Contains("T") || endDate.Contains(":");
                if (!endHasTime)
                {
                    parsedEnd = parsedEnd.Date.AddDays(1).AddTicks(-1); // end-of-day inclusive
                }

                _logger.LogInformation("Parsed filters: Start={Start}, End={End}", parsedStart, parsedEnd);

                var result = await _transactionRepository.GetTransactionDetailsAsync(
                    parsedStart,
                    parsedEnd,
                    operationalShift,
                    tollOperators,
                    laneNames,
                    paymentMethods
                );

                if (result == null || !result.Any())
                {
                    return NotFound("No transactions found for the given filters.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching transaction details.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
