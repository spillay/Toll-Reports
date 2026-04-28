using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models.Traffic.Monthly;
using MIS.Web.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MIS.Web.Controllers
{
    [Authorize]
    public class MonthlyTrafficController : Controller
    {
        private readonly IMonthlyTrafficReportService _trafficService;

        public MonthlyTrafficController(IMonthlyTrafficReportService trafficService)
        {
            _trafficService = trafficService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            int? year = null,
            int? month = null,
            bool operationalMonth = false,
            List<string>? classification = null,   // ✅ checklist
            List<int>? shifts = null)              // ✅ binds from checkboxes
        {
            // 1) Get report (this should return a PageMonthlyTrafficModel with Items)
            var model = await _trafficService.GetTrafficReportAsync(
                year,
                month,
                operationalMonth,
                classification,
                operationalMonth == true ? shifts : null
            );

            model ??= new PageMonthlyTrafficModel();

            // 2) Load filter values
            model.AvailableYears = await (_trafficService?.GetAvailableYearsAsync() ?? Task.FromResult(new List<int>()));
            model.AvailableMonths = year.HasValue
                ? await (_trafficService?.GetAvailableMonthsAsync(year.Value) ?? Task.FromResult(new List<int>()))
                : new List<int>();

            model.AvailableClassifications = await (_trafficService?.GetAvailableClassificationsAsync() ?? Task.FromResult(new List<string>()));

            // 3) Preserve filters
            model.Filters = new MonthlyTrafficInputModel
            {
                Year = year,
                Month = month,
                OperationalMonth = operationalMonth,
                Classifications = classification ?? new List<string>(),
                Shifts = shifts ?? new List<int>()
            };

            return View("~/Views/Traffic/Monthly/Index.cshtml", model);
        }
    }
}
