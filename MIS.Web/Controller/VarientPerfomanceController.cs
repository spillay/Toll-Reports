using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MIS.Web.Models;
using MIS.Web.Models.VarientPerfomance;
using MIS.Web.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MIS.Web.Controllers
{
    public class VarientPerfomanceController : Controller
    {
        private readonly IVarientPerfomanceReportService _reportService;
        private readonly IConfiguration _config;   // 🔥 Needed for dynamic API URL

        public VarientPerfomanceController(
            IVarientPerfomanceReportService reportService,
            IConfiguration config)
        {
            _reportService = reportService;
            _config = config;
        }

        public async Task<IActionResult> VarientPerfomances(
            int page = 1, int pageSize = 10,
            string? shift = null, string? tollOperatorID = null,
            DateTime? startDate = null, DateTime? endDate = null)
        {
            // Default ranges
            startDate ??= DateTime.Today.AddDays(-90);
            endDate ??= DateTime.Today;

            // Convert filters to lists
            var shifts = string.IsNullOrEmpty(shift) ? null : new List<string> { shift };
            var operators = string.IsNullOrEmpty(tollOperatorID) ? null : new List<string> { tollOperatorID };

            // Fetch paginated data
            var data = await _reportService.GetVarientPerfomanceDetailsAsync(
                page, pageSize, startDate.Value, endDate.Value, shifts, operators);

            // TEMP: Hardcoded (will be replaced with API later)
            ViewBag.Shifts = new List<string> { "Shift One", "Shift Two", "Shift Three" };
            ViewBag.TollOperators = new List<string> { "0001", "0002", "0003", "0004", "0005" };

            // Pagination
            int totalPages = data.totalCount > 0
                ? (int)Math.Ceiling((double)data.totalCount / pageSize)
                : 0;

            // =============================================
            //  Build dynamic API URL (this fixes all PCs)
            // =============================================
            string baseUrl = _config["BaseApiUrl:Link"]; // e.g. http://localhost:4567/
            string endpoint = _config["ApiSettings:VarientPerformanceEndpoint"]; // e.g. api/VarientPerformance/details

            ViewData["VarientPerformanceApi"] = $"{baseUrl}{endpoint}";

            // Build Razor model
            var model = new VarientPerfomanceInputModel
            {
                items = data.items ?? new List<VarientPerfomanceModel>(),
                totalCount = data.totalCount,
                page = page,
                pageSize = pageSize,
                totalPages = totalPages,
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                Shift = shift,
                TollOperatorID = tollOperatorID
            };

            return View("Views/VarientPerfomance/Index.cshtml", model);
        }
    }
}
