using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toll.Reporting.Api.Repositories;
using Toll.Reporting.Api.DTOs;

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
        // Example:
        // /api/DailyCashup/details?startDate=2025-01-01&endDate=2025-11-11&shiftIds=1&shiftIds=2&tollOperators=0001&tollOperators=0002
        [HttpGet("details")]
        public async Task<IActionResult> GetDailyCashup(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] List<int>? shiftIds,
            [FromQuery] List<string>? tollOperators,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Fetching daily cashup report from {Start} to {End}, page {Page}, size {Size}",
                startDate, endDate, page, pageSize);

            // Map shift IDs -> short names ("Shift One", "Shift Two", "Shift Three")
            List<string>? operationalShift = null;
            if (shiftIds != null && shiftIds.Any())
            {
                operationalShift = shiftIds
                    .Select(MapShiftIdToDescription)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToList();
            }

            _logger.LogInformation("Filters => ShiftIds: {ShiftIds}, ShiftsMapped: {Shifts}, Operators: {Operators}",
                string.Join(",", shiftIds ?? new List<int>()),
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

            _logger.LogInformation("Total grouped daily cashup records found: {Count}", result.TotalCount);
            return Ok(result);
        }

        // api/DailyCashup/filter-options
        // still uses date range only – front-end can map selected shift IDs itself
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

        // ==========================
        // HELPERS
        // ==========================

        private string MapShiftIdToDescription(int id)
        {
            // Option A: short names
            return id switch
            {
                1 => "Shift One",
                2 => "Shift Two",
                3 => "Shift Three",
                _ => string.Empty
            };
        }
    }
}
