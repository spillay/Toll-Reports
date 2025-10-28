using Microsoft.AspNetCore.Mvc;
using Toll.Reporting.Api.Repositories;

namespace Toll.Reporting.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiscrepancyController : ControllerBase
    {
        private readonly IDiscrepancyRepository _repository;
        private readonly ILogger<DiscrepancyController> _logger;

        public DiscrepancyController(IDiscrepancyRepository repository, ILogger<DiscrepancyController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET /api/discrepancy
        [HttpGet]
        public async Task<IActionResult> GetDiscrepancy(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] List<string>? operationalShift = null,
            [FromQuery] List<string>? tollOperators = null,
            [FromQuery] List<string>? laneNames = null,
            [FromQuery] List<string>? paymentMethods = null,
            [FromQuery] List<string>? takenAction = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                if (startDate == default || endDate == default)
                    return BadRequest("startDate and endDate are required");

                var result = await _repository.GetDiscrepancyAsync(
                    startDate, endDate,
                    operationalShift, tollOperators,
                    laneNames, paymentMethods, takenAction,
                    page, pageSize
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting discrepancy");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
