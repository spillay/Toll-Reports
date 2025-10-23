using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models.Traffic.Monthly;
using MIS.Web.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MIS.Web.Controllers
{
    public class MonthlyTrafficController : Controller
    {
        private readonly IMonthlyTrafficReportService _trafficService;

        public MonthlyTrafficController(IMonthlyTrafficReportService trafficService)
        {
            _trafficService = trafficService;
        }

        // Index now accepts year/month/operationalMonth/class/shifts
        public async Task<IActionResult> Index(
            int? year = null,
            int? month = null,
            bool operationalMonth = false,
            string? classification = null,
            string? shifts = null)
        {
            // Build input model for the view
            var input = new MonthlyTrafficInputModel
            {
                Year = year,
                Month = month,
                OperationalMonth = operationalMonth,
                Classification = classification,
                Shifts = !string.IsNullOrWhiteSpace(shifts)
                            ? shifts.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s)).ToList()
                            : new List<int>()
            };

            // Map service call parameters (note: service expects year/month/operationalMonth/classifications/shifts)
            List<string>? classifications = string.IsNullOrEmpty(classification) ? null : new List<string> { classification };

            var model = await _trafficService.GetTrafficReportAsync(year, month, operationalMonth, classifications, input.Shifts);

            // Pass input and any static classifications (you can replace with dynamic source later)
            ViewBag.InputModel = input;
            ViewBag.Classifications = new List<string> { "Class 1", "Class 2", "Class 3","Class 4", "Class M" };

            return View("~/Views/Traffic/Monthly/Index.cshtml", model);
        }
    }
}
