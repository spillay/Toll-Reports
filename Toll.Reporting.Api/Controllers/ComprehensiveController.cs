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
    public class ComprehensiveController : ControllerBase
    {
        private readonly IComprehensiveRepository _repo;

        public ComprehensiveController(IComprehensiveRepository repo)
        {
            _repo = repo;
        }

        // Example GET:
        // GET /report?startDate=2025/08/19&endDate=2025/08/22&tollOperators=op1,op2&laneNames=LaneA&paymentMethods=Cash
        [HttpGet("report")]
        public async Task<ActionResult<IEnumerable<ComprehensiveDto>>> GetComprehensiveReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] string? operationalShift = null,
            [FromQuery] string? tollOperators = null,
            [FromQuery] string? laneNames = null,
            [FromQuery] string? paymentMethods = null,
            [FromQuery] string? laneDiscountTypes = null,
            [FromQuery] string? classification = null,
            [FromQuery] string? transactionTypes = null)
        {
            try
            {
                // Convert comma-separated to lists (null-safe)
                List<string>? ToList(string? csv) =>
                    string.IsNullOrWhiteSpace(csv)
                        ? null
                        : csv.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();

                var operationalShiftList = ToList(operationalShift);
                var tollOperatorsList = ToList(tollOperators);
                var laneNamesList = ToList(laneNames);
                var paymentMethodsList = ToList(paymentMethods);
                var laneDiscountTypesList = ToList(laneDiscountTypes);
                var classificationList = ToList(classification);
                var transactionTypesList = ToList(transactionTypes);

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

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching comprehensive report: {ex.Message}");
            }
        }
    }
}
