using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models.EndOfDay;
using MIS.Web.Services.Interfaces;

namespace MIS.Web.Controllers
{
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
            var model = new PageEndOfDayModel
            {
                ReportDate = reportDate
            };

            if (reportDate.HasValue)
            {
                model.Rows = await _service.GetEndOfDayAsync(reportDate.Value);
            }

            return View(model);
        }
    }
}
