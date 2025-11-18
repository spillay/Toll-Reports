using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models;
using MIS.Web.Models.TopUp;
using MIS.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MIS.Web.Controllers
{
    public class TopUpController : Controller
    {
        private readonly ITopUpReportService _service;

        public TopUpController(ITopUpReportService service)
        {
            _service = service;
        }

        // =====================================================================
        // ⭐ MAIN UI PAGE (PAGED RESULTS)
        // =====================================================================
        [HttpGet]
        public async Task<IActionResult> Index(TopUpInputModel model)
        {
            // 1️⃣ Default dates
            var start = model.StartDate ?? DateTime.Now.AddDays(-1);
            var end = model.EndDate ?? DateTime.Now;

            // 2️⃣ Operational day range logic
            if (model.OperationalDate == true)
            {
                start = start.Date.AddHours(5.5);                                // 05:30
                end = end.Date.AddDays(1).AddHours(5.5).AddSeconds(-1);         // Next day 05:29:59
            }

            // 3️⃣ Fetch paged data
            var result = await _service.GetTopUpAsync(
                start,
                end,
                model.Shift,
                model.TollOperator,      // ⭐ OPERATOR INCLUDED
                model.Lane,
                model.AccountNumber,
                model.page,
                model.pageSize
            );

            // 4️⃣ Dropdown filters — DISTINCT LISTS FROM ALL DATA
            ViewBag.Shifts = result.items?
                .Select(i => i.Shift)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .OrderBy(s => s)
                .ToList() ?? new List<string>();

            ViewBag.Lanes = result.items?
                .Select(i => i.LaneWorkstation)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Distinct()
                .OrderBy(l => l)
                .ToList() ?? new List<string>();

            ViewBag.AccountNumbers = result.items?
                .Select(i => i.AccountNumber)
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .Distinct()
                .OrderBy(a => a)
                .ToList() ?? new List<string>();

            ViewBag.TollOperators = result.items?
                .Select(i => i.TollOperator)
                .Where(o => !string.IsNullOrWhiteSpace(o))
                .Distinct()
                .OrderBy(o => o)
                .ToList() ?? new List<string>();

            // 5️⃣ Push the page data into the UI model
            model.PageData = result;

            return View(model);
        }

        // =====================================================================
        // ⭐ EXPORT — FULL DATASET (NO PAGING)
        // =====================================================================
        [HttpGet("TopUp/export")]
        public async Task<IActionResult> Export(TopUpInputModel model)
        {
            var start = model.StartDate ?? DateTime.Now.AddDays(-1);
            var end = model.EndDate ?? DateTime.Now;

            if (model.OperationalDate == true)
            {
                start = start.Date.AddHours(5.5);
                end = end.Date.AddDays(1).AddHours(5.5).AddSeconds(-1);
            }

            // FULL data (for Excel & PDF)
            var allData = await _service.GetTopUpFullAsync(
                start,
                end,
                model.Shift,
                model.TollOperator,       // ⭐ OPERATOR INCLUDED
                model.Lane,
                model.AccountNumber
            );

            return Json(allData.items ?? new List<TopUpModel>());
        }
    }
}
