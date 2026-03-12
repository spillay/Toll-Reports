using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toll.Reporting.Api.DTOs;
using Toll.Reporting.Api.Repositories;

namespace Toll.Reporting.Api.Controllers
{
    [ApiController]
    [Route("api/comprehensive")]
    public class ComprehensiveController : ControllerBase
    {
        private readonly IComprehensiveRepository _repo;

        public ComprehensiveController(IComprehensiveRepository repo)
        {
            _repo = repo;
        }
        [HttpGet("options")]
        public async Task<ActionResult<ComprehensiveOptionsDto>> GetOptions()
        {
            var options = await _repo.GetComprehensiveOptionsAsync();
            return Ok(options);
        }

        [HttpGet("report")]
        public async Task<ActionResult<IEnumerable<ComprehensiveDto>>> GetComprehensiveReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,

            //  Multi-select IDs
            [FromQuery] List<byte>? shiftIds = null,
            [FromQuery] List<long>? operatorIds = null,
            [FromQuery] List<int>? laneIds = null,
            [FromQuery] List<byte>? discountTypeIds = null,
            [FromQuery] List<byte>? tollClassIds = null,

            //  PaymentMethod
            [FromQuery] List<byte>? paymentMethodIds = null
        )
        {
            try
            {
                var data = await _repo.GetComprehensiveRepositoryAsync(
                    startDate,
                    endDate,
                    shiftIds,
                    operatorIds,
                    laneIds,
                    discountTypeIds,
                    tollClassIds,
                    paymentMethodIds
                );

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Error fetching comprehensive report.",
                    Details = ex.Message
                });
            }
        }
    }
}