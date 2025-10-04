using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models.Discrepancy;
using MIS.Web.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MIS.Web.Controllers
{
    public class DiscrepancyController : Controller
    {
        private readonly IDiscrepancyReportService _reportService;

        public DiscrepancyController(IDiscrepancyReportService reportService)
        {
            _reportService = reportService;
        }

        // Razor page
        public async Task<IActionResult> Index(DiscrepancyReportViewModel times)
        {
            

            // Fetch data from API via ReportService
            var model = await _reportService.GetDiscrepancyDetailsAsync(times.StartDate, times.EndDate);

            return View(model);
        }
    }
}
