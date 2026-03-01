using Microsoft.AspNetCore.Mvc;
using Toll.Reporting.Api.DTOs;
using Toll.Reporting.Api.Repositories;

namespace Toll.Reporting.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DailyTrafficController : ControllerBase
    {
        private readonly IDailyTrafficRepository _repo;

        public DailyTrafficController(IDailyTrafficRepository repo)
        {
            _repo = repo;
        }


        [HttpGet]
        public async Task<IActionResult> GetDailyTraffic(
        DateTime startDate,
        DateTime endDate,
        [FromQuery] string? classification = null,
        [FromQuery] string? shifts = null,
        [FromQuery] bool? operationalDay = null)
            {
                if (startDate == default || endDate == default)
                    return BadRequest("Please provide valid start and end dates.");

                List<string>? classifications = null;
                if (!string.IsNullOrEmpty(classification))
                    classifications = classification.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                    .Select(s => s.Trim()).ToList();

                List<int>? shiftList = null;
                if (!string.IsNullOrEmpty(shifts))
                    shiftList = shifts.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                      .Select(s => int.TryParse(s.Trim(), out var shift) ? shift : 0)
                                      .Where(s => s >= 1 && s <= 3).ToList();

                var result = await _repo.GetDailyTrafficAsync(startDate, endDate, classifications, shiftList, operationalDay);
                return Ok(result);
            }

        [HttpGet("GetAllClassifications")]
        public async Task<IActionResult> GetAllClassifications()
        {
            var classifications = await _repo.GetAllClassificationsAsync();
            return Ok(classifications);
        }
    }
}
