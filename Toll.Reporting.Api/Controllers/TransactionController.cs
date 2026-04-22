using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toll.Reporting.Api.DTOs;
using Toll.Reporting.Api.Repositories;

namespace Toll.Reporting.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepository _repository;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ITransactionRepository repository, ILogger<TransactionController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves transaction details (paged by default, all data if exportAll = true).
        /// </summary>
        [HttpGet("details")]
        public async Task<IActionResult> GetTransactionDetails(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] List<string>? operationalShift,
            [FromQuery] List<string>? tollOperators,
            [FromQuery] List<string>? laneNames,
            [FromQuery] List<string>? paymentMethods,
            [FromQuery] List<string>? tollCollectorClasses,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool exportAll = false)
        {
            try
            {
                if (startDate == default || endDate == default)
                    return BadRequest("StartDate and EndDate are required.");

                if (exportAll)
                {
                    _logger.LogInformation("Exporting ALL transaction data from {Start} to {End}", startDate, endDate);
                    page = 1;
                    pageSize = int.MaxValue;
                }
                else
                {
                    _logger.LogInformation(
                        "Fetching paginated transactions | Start: {Start} | End: {End} | Page: {Page} | Size: {Size}",
                        startDate, endDate, page, pageSize);
                }

                var result = await _repository.GetTransactionDetailsAsync(
                    startDate,
                    endDate,
                    operationalShift,
                    tollOperators,
                    laneNames,
                    paymentMethods,
                    tollCollectorClasses,
                    page,
                    pageSize
                );

                _logger.LogInformation("Transactions retrieved successfully. Count: {Count}", result.TotalCount);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching transaction details.");
                return StatusCode(500, "An error occurred while retrieving transaction data.");
            }
        }

        /// <summary>
        /// Retrieves filter dropdown options for the transaction report (Shift, Operator, Lane, Payment).
        /// </summary>
        [HttpGet("filter-options")]
        public async Task<IActionResult> GetTransactionFilterOptions(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate == default || endDate == default)
                    return BadRequest("StartDate and EndDate are required.");

                _logger.LogInformation("Fetching transaction filter options for {Start} - {End}", startDate, endDate);

                var options = await _repository.GetTransactionFilterOptionsAsync(startDate, endDate);

                return Ok(options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching transaction filter options.");
                return StatusCode(500, "An error occurred while retrieving filter data.");
            }
        }
    }
}
