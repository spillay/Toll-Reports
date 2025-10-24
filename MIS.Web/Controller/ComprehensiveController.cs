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

        // If you prefer MVC Index to accept filters via form post, you can adjust this.
        // For now it accepts a view model that may contain filter values.
        public async Task<IActionResult> Index(ComprehensiveReportViewModel times)
        {
            // Debug logging (remove or replace with ILogger in production)
            Console.WriteLine($"TransactionType filter: {times.TransactionType}");

            // Convert single-string filters to lists (service expects lists)
            var result = await _reportService.GetComprehensiveDetailsAsync(
                times.StartDate,
                times.EndDate,
                operationalShift: string.IsNullOrWhiteSpace(times.Shift) ? null : new System.Collections.Generic.List<string> { times.Shift },
                tollOperators: string.IsNullOrWhiteSpace(times.TollOperatorID) ? null : new System.Collections.Generic.List<string> { times.TollOperatorID },
                laneNames: string.IsNullOrWhiteSpace(times.LaneName) ? null : new System.Collections.Generic.List<string> { times.LaneName },
                paymentMethods: string.IsNullOrWhiteSpace(times.MethodOfPayment) ? null : new System.Collections.Generic.List<string> { times.MethodOfPayment },
                laneDiscountTypes: string.IsNullOrWhiteSpace(times.DiscountType) ? null : new System.Collections.Generic.List<string> { times.DiscountType },
                classification: string.IsNullOrWhiteSpace(times.ManualTollClass) ? null : new System.Collections.Generic.List<string> { times.ManualTollClass }
            );

            return View("~/Views/Comprehensive/Index.cshtml", result);
        }
    }
}