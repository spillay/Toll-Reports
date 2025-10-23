using Microsoft.AspNetCore.Mvc;
using Toll.Reporting.Api.Repositories;

namespace Toll.Reporting.Api.Controllers
{
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
            [FromQuery] string? classification = null,
            [FromQuery] string? shifts = null)
        {
            List<string>? classifications = null;
            if (!string.IsNullOrWhiteSpace(classification))
                classifications = classification.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                .Select(c => c.Trim())
                                                .ToList();

            List<int>? shiftList = null;
            if (!string.IsNullOrWhiteSpace(shifts))
                shiftList = shifts.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                  .Select(s => int.Parse(s.Trim()))
                                  .ToList();

            var data = await _repo.GetMonthlyTrafficAsync(
                year, month, operationalMonth, classifications, shiftList
            );

            return Ok(data);
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
