using Microsoft.AspNetCore.Mvc;
using Toll.Reporting.Api.Repositories;

namespace Toll.Reporting.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiscrepancyController : ControllerBase
    {
        private readonly IDiscrepancyRepository _repository;
        private readonly ILogger<DiscrepancyController> _logger;

        public DiscrepancyController(IDiscrepancyRepository repository, ILogger<DiscrepancyController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves discrepancy report data (paged by default, all data if exportAll = true).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetDiscrepancy(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] List<string>? operationalShift = null,
            [FromQuery] List<string>? tollOperators = null,
            [FromQuery] List<string>? laneNames = null,
            [FromQuery] List<string>? paymentMethods = null,
            [FromQuery] List<string>? takenAction = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] bool exportAll = false)
        {
            try
            {
                //  Validate dates
                if (startDate == default || endDate == default)
                    return BadRequest("startDate and endDate are required.");

                //  Normalize paging
                page = page <= 0 ? 1 : page;
                pageSize = pageSize <= 0 ? 50 : pageSize;

                // Export mode (fetch everything)
                if (exportAll)
                {
                    _logger.LogInformation("Exporting ALL discrepancy data from {Start} to {End}", startDate, endDate);
                    page = 1;
                    pageSize = int.MaxValue;
                }
                else
                {
                    _logger.LogInformation(
                        "Fetching paginated discrepancies | Start: {Start} | End: {End} | Page: {Page} | Size: {Size}",
                        startDate, endDate, page, pageSize);
                }

                var result = await _repository.GetDiscrepancyAsync(
                    startDate,
                    endDate,
                    operationalShift,
                    tollOperators,
                    laneNames,
                    paymentMethods,
                    takenAction,
                    page,
                    pageSize
                );

                _logger.LogInformation("✅ Discrepancy data retrieved. Count: {Count}", result.TotalCount);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while getting discrepancy data.");
                return StatusCode(500, "An error occurred while retrieving discrepancy data.");
            }
        }

        /// <summary>
        /// Retrieves filter checklist options for discrepancy report (Shift, Operator, Lane, Payment, TakenAction).
        /// Returns ALL values from DB (used or not used).
        /// </summary>
        [HttpGet("filter-options")]
        public async Task<IActionResult> GetDiscrepancyFilterOptions(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                
                if (startDate == default || endDate == default)
                    return BadRequest("startDate and endDate are required.");

                _logger.LogInformation("Fetching discrepancy filter options for {Start} - {End}", startDate, endDate);

                var options = await _repository.GetDiscrepancyFilterOptionsAsync(startDate, endDate);

                return Ok(options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while getting discrepancy filter options.");
                return StatusCode(500, "An error occurred while retrieving discrepancy filter options.");
            }
        }
    }
}