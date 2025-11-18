using Microsoft.AspNetCore.Mvc;
using Toll.Reporting.Api.Repositories;
using Toll.Reporting.Api.Repositories.Interfaces;

namespace Toll.Reporting.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopUpController : ControllerBase
    {
        private readonly ITopUpRepository _repo;

        public TopUpController(ITopUpRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetTopUps(
            DateTime startDate,
            DateTime endDate,
            string? shift = null,
            string? operatorId = null,
            string? lane = null,
            string? accountNumber = null,
            int page = 1,
            int pageSize = 30)
        {
            var result = await _repo.GetTopUpsAsync(
                startDate, endDate,
                shift, operatorId, lane, accountNumber,
                page, pageSize
            );

            return Ok(result);
        }
    }
}
