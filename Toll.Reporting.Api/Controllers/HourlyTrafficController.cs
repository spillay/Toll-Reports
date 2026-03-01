using Microsoft.AspNetCore.Mvc;
using Toll.Reporting.Api.DTOs;
using Toll.Reporting.Api.Repositories;

namespace Toll.Reporting.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HourlyTrafficController : ControllerBase
    {
        private readonly IHourlyTrafficRepository _repo;

        public HourlyTrafficController(IHourlyTrafficRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("GetHourlyTrafficByDate")]
        public async Task<IActionResult> GetHourlyTrafficByDate(
         DateTime startDate,
         DateTime endDate,
         [FromQuery] bool operationalDay = false, 
         [FromQuery] string? classification = null,
         [FromQuery] string? shifts = null)
        {
            if (startDate == default)
                return BadRequest("Please provide a valid date.");

            // Split classifications if multiple passed
            List<string>? classifications = null;
            if (!string.IsNullOrEmpty(classification))
            {
                classifications = classification
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToList();
            }

            List<int>? shiftList = null;
            if (!string.IsNullOrEmpty(shifts))
            {
                shiftList = shifts
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => int.TryParse(s.Trim(), out var shift) ? shift : 0)
                    .Where(s => s >= 1 && s <= 3)
                    .ToList();

                if (!shiftList.Any())
                    shiftList = null;
            }

            var result = await _repo.GetHourlyTrafficForSingleDayAsync(
                startDate,
                endDate,
                classifications,
                shiftList,
                operationalDay
            );

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
