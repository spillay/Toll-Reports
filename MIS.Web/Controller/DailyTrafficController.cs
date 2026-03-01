using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models.Traffic.Daily;
using MIS.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MIS.Web.Controllers
{
    [Authorize]
    public class DailyTrafficController : Controller
    {
        private readonly IDailyTrafficReportService _trafficService;

        public DailyTrafficController(IDailyTrafficReportService trafficService)
        {
            _trafficService = trafficService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            DateTime? startDate = null,
            DateTime? endDate = null,
            List<string>? classification = null,    // ✅ checklist binds here
            List<int>? shifts = null,
            bool operationalDay = false)
        {
            var now = DateTime.Now;

            var start = startDate ?? now.AddDays(-7);
            var end = endDate ?? now;

            // ✅ If nothing selected -> treat as "All" (null)
            var selectedClasses = (classification != null && classification.Any())
                ? classification
                    .Select(x => x?.Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList()
                : null;

            // ✅ Only send shifts if operational day is enabled
            var shiftsToSend = operationalDay ? (shifts ?? new List<int>()) : null;

            // 1) Get report data
            var pageModel = await _trafficService.GetTrafficReportAsync(
                start,
                end,
                selectedClasses,
                shiftsToSend,
                operationalDay);

            pageModel ??= new PageDailyTrafficModel();

            // 2) Load lookup values (DB/API)
            var allClasses = await _trafficService.GetAllClassificationsAsync();
            pageModel.Classifications = allClasses ?? new List<string>();

            // 3) Preserve filters for UI
            pageModel.Filters = new DailyTrafficInputModel
            {
                StartDate = start,
                EndDate = end,
                // ✅ store the selected list for checkbox re-checking
                ClassificationList = selectedClasses ?? new List<string>(),
                Shifts = shifts ?? new List<int>(),
                OperationalDay = operationalDay
            };

            return View("~/Views/Traffic/Daily/Index.cshtml", pageModel);
        }
    }
}