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
        public async Task<IActionResult> Index(DateTime? reportDate, int? shiftId = null)
        {
            var selectedDate = reportDate?.Date ?? DateTime.Today;

            var startDate = selectedDate;
            var endDate = selectedDate;

            var model = new PageEndOfDayModel
            {
                ReportDate = selectedDate,
                StartDate = startDate,
                EndDate = endDate,
                ShiftId = shiftId,
                Report = await _service.GetEndOfDayAsync(startDate, endDate, shiftId)
                         ?? new EndOfDayReportViewModel()
            };

            return View(model);
        }
    }
}
