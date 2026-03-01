using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models.DailyCashup;
using MIS.Web.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MIS.Web.Controllers
{
    [Authorize]
    public class DailyCashupController : Controller
    {
        private readonly IDailyCashupReportService _service;

        public DailyCashupController(IDailyCashupReportService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            DateTime? startDate,
            DateTime? endDate,
            string? OperationalShift,
            string? TollOperators,
            int page = 1,
            int pageSize = 20)
        {
            var start = startDate ?? DateTime.Now.AddDays(-7);
            var end = endDate ?? DateTime.Now;

            var operationalShifts = string.IsNullOrEmpty(OperationalShift)
                ? new List<string>()
                : new List<string> { OperationalShift };

            var tollOperators = string.IsNullOrEmpty(TollOperators)
                ? new List<string>()
                : new List<string> { TollOperators };

            // 1️⃣ Fetch report data
            var data = await _service.GetDailyCashupAsync(start, end, operationalShifts, tollOperators, page, pageSize);

            // 2️⃣ Fetch dropdown data directly from API
            var shifts = await _service.GetShiftsAsync();
            var operators = await _service.GetTollOperatorsAsync();

            // 3️⃣ Set ViewBags for Razor rendering
            ViewBag.Shifts = shifts ?? new List<string>();
            ViewBag.TollOperators = operators ?? new List<string>();

            // 4️⃣ Persist selected filters
            data.StartDate = start;
            data.EndDate = end;

            return View(data);
        }
    }
}
