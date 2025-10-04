using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models.Transaction;
using MIS.Web.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MIS.Web.Controllers
{
    public class TransactionController : Controller
    {
        private readonly IReportService _reportService;

        public TransactionController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // Razor page
        public async Task<IActionResult> Index(TransactionReportViewModel times)
        {
            //var sDate = startDate ?? new DateTime(2025/08/19);
            //var eDate = endDate ?? new DateTime(2025/08/22);

            // Fetch data from API via ReportService
            var model = await _reportService.GetTransactionDetailsAsync(times.StartDate, times.EndDate);

            return View(model);
        }
    }
}
