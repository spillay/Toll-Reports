using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Toll.Reporting.Api.Repositories;
using MIS.Models;
using Toll.Reporting.Api.DTOs;
using Microsoft.Extensions.Logging;

namespace Toll.Reporting.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LookupController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<LookupController> _logger;

        public LookupController(ITransactionRepository transactionRepository, ILogger<LookupController> logger)
        {
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        /// <summary>
        /// Get distinct shift descriptions for dropdown filters.
        /// </summary>
        [HttpGet("shifts")]
        public async Task<IActionResult> GetShifts()
        {
            try
            {
                var result = await _transactionRepository.GetShiftsAsync();
                if (!result.Any())
                    return NotFound("No shifts found.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching shifts.");
                return StatusCode(500, "An error occurred while retrieving shifts.");
            }
        }

        /// <summary>
        /// Get distinct toll operator usernames for dropdown filters.
        /// </summary>
        [HttpGet("operators")]
        public async Task<IActionResult> GetTollOperators()
        {
            try
            {
                var result = await _transactionRepository.GetTollOperatorsAsync();
                if (!result.Any())
                    return NotFound("No toll operators found.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching toll operators.");
                return StatusCode(500, "An error occurred while retrieving toll operators.");
            }
        }

        /// <summary>
        /// Get distinct lane names for dropdown filters.
        /// </summary>
        [HttpGet("lanes")]
        public async Task<IActionResult> GetLanes()
        {
            try
            {
                var result = await _transactionRepository.GetLanesAsync();
                if (!result.Any())
                    return NotFound("No lanes found.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching lanes.");
                return StatusCode(500, "An error occurred while retrieving lanes.");
            }
        }

        /// <summary>
        /// Get distinct payment methods for dropdown filters.
        /// </summary>
        [HttpGet("payment-methods")]
        public async Task<IActionResult> GetPaymentMethods()
        {
            try
            {
                var result = await _transactionRepository.GetPaymentMethodsAsync();
                if (!result.Any())
                    return NotFound("No payment methods found.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                // ILogger.LogError(ex, "Error fetching payment methods.");
                return StatusCode(500, "An error occurred while retrieving payment methods.");
            }
        }
    }
}

