using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Toll.Reporting.Api.Repositories;
using MIS.Models;
using Toll.Reporting.Api.DTOs;

namespace Toll.Reporting.Api.Controllers
{
    public class DiscrepancyController : ControllerBase
    {
        private readonly IDiscrepancyRepository _discipencyRepository;
        private readonly ILogger<DiscrepancyController> _logger;

        // Constructor with Dependency Injection
        public DiscrepancyController(IDiscrepancyRepository discrepancyRepository, ILogger<DiscrepancyController> logger)
        {
            _discipencyRepository = discrepancyRepository;
            _logger = logger;
        }

        /// <summary>
        /// Get Transaction Details Report with optional filters.
        /// </summary>
        /// <param name="startDate">Start date (yyyy-MM-dd)</param>
        /// <param name="endDate">End date (yyyy-MM-dd)</param>
        /// <param name="operationalShift">Optional list of shift names</param>
        /// <param name="tollOperators">Optional list of toll operators</param>
        /// <param name="laneNames">Optional list of lane names</param>
        /// <param name="tanenaction">Optional list of payment methods</param>
        /// <returns>Filtered TransactionDetailsDto records</returns>
        [HttpGet("discrepancy")]
        public async Task<IActionResult> GetDiscrepancy(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] List<string>? operationalShift = null,
            [FromQuery] List<string>? tollOperators = null,
            [FromQuery] List<string>? laneNames = null,
            [FromQuery] List<string>? takenaction = null)
        {
            try
            {
                if (startDate == default || endDate == default)
                {
                    return BadRequest("StartDate and EndDate are required.");
                }

                var result = await _discipencyRepository.GetDiscrepancyAsync(
                    startDate,
                    endDate,
                    operationalShift,
                    tollOperators,
                    laneNames,
                    takenaction
                );

                if (result == null || !result.Any())
                {
                    return NotFound("No discripancies found for the given filters.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching discepancies details.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}