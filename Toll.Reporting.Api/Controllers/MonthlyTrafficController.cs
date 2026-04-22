using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toll.Reporting.Api.Repositories;

namespace Toll.Reporting.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MonthlyTrafficController : ControllerBase
    {
        private readonly IMonthlyTrafficRepository _repo;

        public MonthlyTrafficController(IMonthlyTrafficRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetMonthlyTraffic(
        [FromQuery] int? year = null,
        [FromQuery] int? month = null,
        [FromQuery] bool? operationalMonth = null,
        [FromQuery] List<string>? classification = null,   
        [FromQuery] string? shifts = null)
        {
            // ✅ Shift list still comma-separated (same as before)
            List<int>? shiftList = null;
            if (!string.IsNullOrWhiteSpace(shifts))
            {
                shiftList = shifts.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                  .Select(s => int.TryParse(s.Trim(), out var v) ? v : (int?)null)
                                  .Where(v => v.HasValue)
                                  .Select(v => v!.Value)
                                  .Distinct()
                                  .ToList();
            }

            var data = await _repo.GetMonthlyTrafficAsync(
                year, month, operationalMonth, classification, shiftList
            );

            return Ok(data);
        }
        [HttpGet("classifications")]
        public async Task<IActionResult> GetClassifications()
        {
            var classes = await _repo.GetAvailableClassificationsAsync();
            return Ok(classes);
        }

        [HttpGet("years")]
        public async Task<IActionResult> GetAvailableYears()
        {
            var years = await _repo.GetAvailableYearsAsync();
            return Ok(years);
        }

        [HttpGet("months/{year}")]
        public async Task<IActionResult> GetAvailableMonths(int year)
        {
            var months = await _repo.GetAvailableMonthsAsync(year);
            return Ok(months);
        }
    }
}
