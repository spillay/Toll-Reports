using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toll.Reporting.Api.Repositories;

namespace Toll.Reporting.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TopUpController : ControllerBase
    {
        private readonly ITopUpRepository _repo;

        public TopUpController(ITopUpRepository repo)
        {
            _repo = repo;
        }

        // 1. FILTER OPTIONS ENDPOINT (for checkbox lists)
        [HttpGet("filter-options")]
        public async Task<IActionResult> GetFilterOptions()
        {
            var options = await _repo.GetTopUpFilterOptionsAsync();
            return Ok(options);
        }


        // 2. REPORT ENDPOINT (checkbox multi-select supported)
        [HttpGet("details")]
        public async Task<IActionResult> GetTopUps(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,

            // checkbox multi-select binding
            [FromQuery] List<string>? shifts = null,
            [FromQuery] List<string>? operatorIds = null,
            [FromQuery] List<string>? lanes = null,
            [FromQuery] List<string>? paymentMethods = null,

            [FromQuery] string? accountNumber = null,

            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 30
        )
        {
            var result = await _repo.GetTopUpsAsync(
                startDate,
                endDate,
                shifts,
                operatorIds,
                lanes,
                paymentMethods,
                accountNumber,
                page,
                pageSize
            );

            return Ok(result);
        }
    }
}