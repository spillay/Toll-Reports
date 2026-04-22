using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toll.Reporting.Api.Repositories;
using Toll.Reporting.Api.Repositories.Interfaces;

namespace Toll.Reporting.Api.Controllers
{
    [Authorize]
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

        [HttpGet("details")]
        public async Task<IActionResult> GetDailyCashup(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] List<int>? shiftIds,
            [FromQuery] List<long>? systemUserIds,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _repo.GetDailyCashupAsync(
                startDate,
                endDate,
                shiftIds,
                systemUserIds,
                page,
                pageSize);

            return Ok(result);
        }

        [HttpGet("filters")]
        public async Task<IActionResult> GetFilters()
        {
            var options = await _repo.GetDailyCashupFilterOptionsAsync();
            return Ok(options);
        }
    }
}