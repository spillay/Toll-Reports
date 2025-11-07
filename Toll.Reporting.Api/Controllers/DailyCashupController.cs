using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Toll.Reporting.Api.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Toll.Reporting.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DailyCashupController : ControllerBase
    {
        private readonly IDailyCashupRepository _repo;
        private readonly ILogger<DailyCashupController> _logger;

        public DailyCashupController(IDailyCashupRepository repo, ILogger<DailyCashupController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        // GET: api/DailyCashup/operators
        [HttpGet("operators")]
        public async Task<IActionResult> GetTollOperators()
        {
            var operators = await _repo.GetTollOperatorsAsync();
            return Ok(operators);
        }

        // GET: api/DailyCashup/shifts
        [HttpGet("shifts")]
        public async Task<IActionResult> GetShifts()
        {
            var shifts = await _repo.GetShiftsAsync();
            return Ok(shifts);
        }


        // GET: api/DailyCashup/details
        [HttpGet("details")]
        public async Task<IActionResult> GetDailyCashup(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] List<string>? operationalShift,
            [FromQuery] List<string>? tollOperators,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Fetching daily cashup report from {Start} to {End}, page {Page}, size {Size}",
                startDate, endDate, page, pageSize);

            _logger.LogInformation("Filters => Shifts: {Shifts}, Operators: {Operators}",
                string.Join(",", operationalShift ?? new List<string>()),
                string.Join(",", tollOperators ?? new List<string>()));

            var result = await _repo.GetDailyCashupAsync(
                startDate,
                endDate,
                operationalShift,
                tollOperators,
                page,
                pageSize
            );

            _logger.LogInformation("Total daily cashup records found: {Count}", result.TotalCount);
            return Ok(result);
        }

        // api/DailyCashup/filter-options
        [HttpGet("filter-options")]
        public async Task<IActionResult> GetDailyCashupFilterOptions(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] List<string>? operationalShift,
            [FromQuery] List<string>? tollOperators)
        {
            var options = await _repo.GetDailyCashupFilterOptionsAsync(
                startDate, endDate, operationalShift, tollOperators);

            return Ok(options);
        }
    }
}
