using Microsoft.AspNetCore.Mvc;
using Toll.Reporting.Api.DTOs;
using Toll.Reporting.Api.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toll.Reporting.Api.Controllers
{
    [ApiController]
    [Route("api/comprehensive")]
    public class ComprehensiveController : ControllerBase
    {
        private readonly IComprehensiveRepository _repo;

        public ComprehensiveController(IComprehensiveRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Retrieves the comprehensive report for the specified filters and date range.
        /// </summary>
        [HttpGet("report")]
        public async Task<ActionResult<IEnumerable<ComprehensiveDto>>> GetComprehensiveReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] string? operationalShift = null,
            [FromQuery] string? tollOperators = null,      // maps to SystemUser.Username (Operator)
            [FromQuery] string? laneNames = null,
            [FromQuery] string? laneDiscountTypes = null,  // discount descriptions
            [FromQuery] string? classification = null,
            [FromQuery] string? paymentMethods = null,
            [FromQuery] string? transactionTypes = null)
        {
            try
            {
                // --- Utility: convert comma-separated values to list safely ---
                static List<string>? ToList(string? csv) =>
                    string.IsNullOrWhiteSpace(csv)
                        ? null
                        : csv.Split(',', StringSplitOptions.RemoveEmptyEntries)
                             .Select(s => s.Trim())
                             .ToList();

                // --- Parse incoming filters ---
                var operationalShiftList = ToList(operationalShift);
                var tollOperatorsList = ToList(tollOperators);
                var laneNamesList = ToList(laneNames);
                var laneDiscountTypesList = ToList(laneDiscountTypes);
                var classificationList = ToList(classification);
                var paymentMethodsList = ToList(paymentMethods);
                var transactionTypesList = ToList(transactionTypes);

                // --- Repository call (correct parameter order) ---
                var data = await _repo.GetComprehensiveRepositoryAsync(
                    startDate,
                    endDate,
                    operationalShiftList,
                    tollOperatorsList,      
                    laneNamesList,
                    laneDiscountTypesList,  
                    classificationList,
                    paymentMethodsList,
                    transactionTypesList
                );

                // --- Return data ---
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Error fetching comprehensive report.",
                    Details = ex.Message
                });
            }
        }
    }
}
