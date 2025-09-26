using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Toll.Reporting.Api.Repositories;
using MIS.Models;

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
        public async Task<ActionResult<IEnumerable<Transaction>>> GetComprehensiveReport()
        {
            try
            {
                var data = await _repo.GetComprehensiveReportAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching comprehensive report: {ex.Message}");
            }
        }
    }
}