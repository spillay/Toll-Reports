using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Toll.Reporting.Api.Repositories;
using MIS.Models;
using Toll.Reporting.Api.DTOs;
//using Toll.Reporting.Api.Interfaces;

namespace Toll.Reporting.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<TransactionController> _logger;

        // Constructor with Dependency Injection
        public TransactionController(ITransactionRepository transactionRepository, ILogger<TransactionController> logger)
        {
            _transactionRepository = transactionRepository;
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
        /// <param name="paymentMethods">Optional list of payment methods</param>
        /// <returns>Filtered TransactionDetailsDto records</returns>
        [HttpGet("details")]
        public async Task<IActionResult> GetTransactionDetails(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] List<string>? operationalShift = null,
            [FromQuery] List<string>? tollOperators = null,
            [FromQuery] List<string>? laneNames = null,
            [FromQuery] List<string>? paymentMethods = null)
        {
            try
            {
                if (startDate == default || endDate == default)
                {
                    return BadRequest("StartDate and EndDate are required.");
                }

                var result = await _transactionRepository.GetTransactionDetailsAsync(
                    startDate,
                    endDate,
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
