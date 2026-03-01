using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models.EndOfDay;
using MIS.Web.Services.Interfaces;

namespace MIS.Web.Controllers
{
    [Authorize]
    public class EndOfDayReportController : Controller
    {
        private readonly IEndOfDayReportService _service;

        public EndOfDayReportController(IEndOfDayReportService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? reportDate)
        {
            // default to today
            DateTime date = reportDate ?? DateTime.Today;

            // Operational day boundaries
            DateTime startDate = date.Date.AddHours(5).AddMinutes(30);        // 05:30 AM same day
            DateTime endDate = date.Date.AddDays(1).AddHours(5).AddMinutes(29); // 05:29 AM next day

            var model = new PageEndOfDayModel
            {
                StartDate = startDate,
                EndDate = endDate,
                Report = await _service.GetEndOfDayAsync(startDate, endDate)
            };

            return View(model);
        }

    }
}
