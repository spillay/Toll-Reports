using Microsoft.AspNetCore.Mvc;
using Toll.Reporting.Api.Repositories;
using System;
using System.Threading.Tasks;

namespace Toll.Reporting.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopUpController : ControllerBase
    {
        private readonly ITopUpRepository _repository;

        public TopUpController(ITopUpRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetTopUps(
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? operatorId = null,
            string? lane = null,
            string? shift = null,
            string? accountNumber = null,
            bool? operationalDate = null,
            int page = 1,
            int pageSize = 50)
        {
            try
            {
                // ✅ Safe defaults (past 7 days if not provided)
                var fromDate = startDate ?? DateTime.Now.AddDays(-7);
                var toDate = endDate ?? DateTime.Now;

                // ✅ Clamp any invalid values (before SQL's min date)
                if (fromDate < new DateTime(1753, 1, 1))
                    fromDate = new DateTime(1753, 1, 1);
                if (toDate < new DateTime(1753, 1, 1))
                    toDate = new DateTime(1753, 1, 1);

                // ✅ Ensure endDate is after startDate
                if (toDate < fromDate)
                    toDate = fromDate.AddDays(1);

                var result = await _repository.GetTopUpsAsync(
                    fromDate,
                    toDate,
                    operatorId,
                    lane,
                    shift,
                    accountNumber,
                    operationalDate,
                    page,
                    pageSize
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "An error occurred while fetching the Top-Up report.",
                    details = ex.Message
                });
            }
        }
    }
}
