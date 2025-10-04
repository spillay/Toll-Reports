using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MIS.Models;
using Toll.Reporting.Api.DTOs;
using Toll.Reporting.Api.Repositories;

namespace Toll.Reporting.Api.Controllers
{
    public class ComprehensiveController : Controller
    {
        private readonly IComprehensiveRepository _repo;

        public ComprehensiveController(IComprehensiveRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("report")]
        public async Task<ActionResult<IEnumerable<ComprehensiveDto>>> GetComprehensiveReport([FromQuery] ComprehensiveDto info)
        {
            try
            {
                var data = await _repo.GetComprehensiveRepositoryAsync(info.StartDate, info.EndDate, info.MethodOfPayment);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching comprehensive report: {ex.Message}");
            }
        }

    }
}