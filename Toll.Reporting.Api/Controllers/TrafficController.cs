using Microsoft.AspNetCore.Mvc;
using Toll.Reporting.Api.DTOs;
using Toll.Reporting.Api.Enums;
using Toll.Reporting.Api.Repositories;

namespace Toll.Reporting.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrafficController : ControllerBase
    {
        private readonly ITrafficRepository _trafficRepository;

        public TrafficController(ITrafficRepository trafficRepository)
        {
            _trafficRepository = trafficRepository;
        }

        [HttpGet("GetTraffic")]
        public async Task<IActionResult> GetTraffic(
            DateTime startDate,
            DateTime endDate,
            ReportViewType viewType,
            [FromQuery] string? classification,  // comma-separated
            int page = 1,
            int pageSize = 10)
        {
            if (startDate > endDate)
                return BadRequest("Start date cannot be after end date.");

            // Split comma-separated classification
            List<string>? classifications = null;
            if (!string.IsNullOrEmpty(classification))
            {
                classifications = classification
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(c => c.Trim())
                    .ToList();
            }

            var result = await _trafficRepository.GetTrafficAsync(
                startDate, endDate, viewType, classifications, page, pageSize);

            return Ok(result);
        }
    }
}
