using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toll.Reporting.Api.Models.AvcAccuracy;
using Toll.Reporting.Api.Repositories;

namespace Toll.Reporting.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AvcAccuracyController : ControllerBase
    {
        private readonly IAvcAccuracyRepository _service;

        public AvcAccuracyController(IAvcAccuracyRepository service)
        {
            _service = service;
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetDetails(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] List<int>? shiftIds = null,
            [FromQuery] List<int>? laneIds = null,
            [FromQuery] List<int>? classIds = null)
        {
            var request = new AvcAccuracyRequest
            {
                StartDate = startDate,
                EndDate = endDate,
                ShiftIds = shiftIds ?? new List<int>(),
                LaneIds = laneIds ?? new List<int>(),
                ClassIds = classIds ?? new List<int>()
            };

            var result = await _service.GetBaseDataAsync(request);
            return Ok(result);
        }

        [HttpGet("filter-options")]
        public async Task<IActionResult> GetFilterOptions()
        {
            var result = await _service.GetFilterOptionsAsync();
            return Ok(result);
        }
    }
}