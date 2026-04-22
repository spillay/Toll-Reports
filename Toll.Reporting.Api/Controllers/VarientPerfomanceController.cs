using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toll.Reporting.Api.Repositories;

namespace Toll.Reporting.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class VarientPerformanceController : ControllerBase
    {
        private readonly IVarientPerformanceRepository _repo;
        private readonly ILogger<VarientPerformanceController> _logger;

        public VarientPerformanceController(IVarientPerformanceRepository repo, ILogger<VarientPerformanceController> logger)
        {
            _repo = repo;
            _logger = logger;
        }
        [HttpGet("shifts")]
        public async Task<IActionResult> GetShifts()
        {
            var shifts = await _repo.GetShiftsAsync();
            return Ok(shifts);
        }

        [HttpGet("operators")]
        public async Task<IActionResult> GetOperators()
        {
            var ops = await _repo.GetTollOperatorsAsync();
            return Ok(ops);
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetVarientPerformanceDetails(
            DateTime startDate,
            DateTime endDate,
            [FromQuery] List<string>? operationalShift,
            [FromQuery] List<string>? tollOperators,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Fetching variant performance from {Start} to {End}, page {Page}, size {Size}", startDate, endDate, page, pageSize);
            _logger.LogInformation("Filters => Shifts: {Shifts}, Operators: {Operators}",
                string.Join(",", operationalShift ?? new List<string>()),
                string.Join(",", tollOperators ?? new List<string>()));

            var result = await _repo.GetVarientPerformanceAsync(
                startDate,
                endDate,
                operationalShift,
                tollOperators,
                page,
                pageSize
            );

            _logger.LogInformation("Total records found: {Count}", result.TotalCount);
            return Ok(result);
        }
    }
}
