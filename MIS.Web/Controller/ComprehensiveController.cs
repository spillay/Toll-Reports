using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models.Comprehensive;
using MIS.Web.Services;
using System;
using System.Threading.Tasks;

namespace MIS.Web.Controllers
{
    public class ComprehensiveController : Controller
    {
        private readonly IComprehensiveReportService _reportService;

        public ComprehensiveController(IComprehensiveReportService reportService)
        {
            _reportService = reportService;
        }

        // Razor Page: Index
        public async Task<IActionResult> Index(ComprehensiveReportViewModel times)
        {
            // Debug logging
            Console.WriteLine(times.methodOfPayment);

            // Fetch data from service
            var model = await _reportService.GetComprehensiveDetailsAsync(times.StartDate, times.EndDate);

            Console.WriteLine(times.methodOfPayment);
            return View(model);
        }
    }
}
