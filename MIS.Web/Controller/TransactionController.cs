using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models;
using MIS.Web.Models.Transaction;
using MIS.Web.Services;


namespace MIS.Web.Controllers
{
    public class TransactionController : Controller
    {
        private readonly IReportService _reportService;

        public TransactionController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<IActionResult> Transaction(TransactionInputModel model)
        {
            try
            {
                // Default filters
                model.StartDate = model.StartDate == default ? DateTime.Today.AddDays(-30) : model.StartDate;
                model.EndDate = model.EndDate == default ? DateTime.Today : model.EndDate;
                model.page = model.page <= 0 ? 1 : model.page;
                model.pageSize = model.pageSize <= 0 ? 50 : model.pageSize;

                // Fetch transaction data from API
                var data = await _reportService.GetTransactionDetailsAsync(model);

                // Fetch dropdown filters
                var filters = await _reportService.GetTransactionFilterOptionsAsync(model);

                // Set dropdown values in ViewBag
                ViewBag.Shifts = filters?.Shifts ?? new List<string>();
                ViewBag.TollOperators = filters?.TollOperators ?? new List<string>();
                ViewBag.Lanes = filters?.Lanes ?? new List<string>();
                ViewBag.PaymentMethods = filters?.PaymentMethods ?? new List<string>();

                // Merge pagination and filters into model
                model.items = data.items;
                model.totalCount = data.totalCount;
                model.totalPages = data.totalPages;

                return View("Views/Transaction/Index.cshtml", model);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ ERROR: " + ex.Message);
                return View("Views/Transaction/Index.cshtml", new TransactionInputModel
                {
                    items = new List<TransactionModel>()
                });
            }
        }
    }
}
