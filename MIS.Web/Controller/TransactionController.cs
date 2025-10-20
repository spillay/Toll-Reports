using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models;
using MIS.Web.Models.Transaction;
using MIS.Web.Services;
using System;
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

        public IActionResult Index()
        {
            return View("Views/Transaction/Index.cshtml", new TransactionInputModel());
        }

        public async Task<IActionResult> Transaction(
            int page = 1,
            int pageSize = 10,
            string? lane_Nr = null,
            string? TollOperatorID = null,
            string? Shift = null,
            string? PaymentMethod = null,
            DateTime? StartDate = null,
            DateTime? EndDate = null)
        {
            var model = new TransactionInputModel
            {
                page = page,
                pageSize = pageSize,
                lane_Nr = lane_Nr,
                TollOperatorID = TollOperatorID,
                Shift = Shift,
                PaymentMethod = PaymentMethod,
                StartDate = StartDate ?? DateTime.Today.AddDays(-30),
                EndDate = EndDate ?? DateTime.Today
            };

            var data = await _reportService.GetTransactionDetailsAsync(model);

            // Extract distinct values for dropdowns from the fetched data
            ViewBag.PaymentMethods = data.items?.Select(t => t.method_of_Payment)
                                               .Where(p => !string.IsNullOrEmpty(p))
                                               .Distinct()
                                               .ToList() ?? new List<string>();

            ViewBag.Shifts = data.items?.Select(t => t.operational_Shift)
                                        .Where(s => !string.IsNullOrEmpty(s))
                                        .Distinct()
                                        .ToList() ?? new List<string>();

            ViewBag.TollOperators = data.items?.Select(t => t.toll_Operator_ID)
                                              .Where(o => !string.IsNullOrEmpty(o))
                                              .Distinct()
                                              .ToList() ?? new List<string>();

            ViewBag.Lanes = data.items?.Select(t => t.lane_Nr)
                                       .Where(l => !string.IsNullOrEmpty(l))
                                       .Distinct()
                                       .ToList() ?? new List<string>();


            return View("Views/Transaction/Index.cshtml", data);
        }
    }
}
