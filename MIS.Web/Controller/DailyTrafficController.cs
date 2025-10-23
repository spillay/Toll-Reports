using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models.Traffic.Daily;
using MIS.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MIS.Web.Controllers
{
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
            string? classification = null,
            List<int> shifts = null,
            bool operationalDay = false)
        {
            var start = startDate ?? DateTime.UtcNow.AddDays(-7);
            var end = endDate ?? DateTime.UtcNow;

            List<string>? classifications = !string.IsNullOrEmpty(classification)
                ? new List<string> { classification }
                : null;

            List<int>? shiftsToSend = operationalDay ? (shifts ?? new List<int>()) : null;

            // Fetch traffic data from service
            var pageModel = await _trafficService.GetTrafficReportAsync(
                start, end, classifications, shiftsToSend, operationalDay);

            // Persist filter selections for the view
            pageModel.Filters = new DailyTrafficInputModel
            {
                StartDate = start,
                EndDate = end,
                Classification = classification,
                Shifts = shifts ?? new List<int>(),
                OperationalDay = operationalDay
            };

            // Populate classifications dynamically from the data
            pageModel.Classifications = pageModel.Items
                .Select(x => x.Classification ?? "Unknown")
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            // If no classifications from data, use default ones
            if (!pageModel.Classifications.Any())
            {
                pageModel.Classifications = new List<string> { "Class 1", "Class 2", "Class 4", "Class M" };
            }

            return View("~/Views/Traffic/Daily/Index.cshtml", pageModel);
        }
    }
}