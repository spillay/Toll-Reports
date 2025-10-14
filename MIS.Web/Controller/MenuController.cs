
using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models;
using MIS.Web.Models.Transaction;
using MIS.Web.Services;

namespace MIS.Web.Controllers
{
  
    public class MenuController : Controller
    {
        private readonly IReportService _reportService;

        public MenuController(IReportService reportService)
        {
            _reportService = reportService;
        }
        public async Task<IActionResult> Transaction(int page = 1, int pageSize = 10)
        {
            // For now, we use static date range (you can later make this dynamic)
            var startDate = DateTime.Parse("08/08/2025");
            var endDate = DateTime.Parse("09/09/2025");

            // Fetch paginated data from your report service
            var data = await _reportService.GetTransactionDetailsAsync(page, pageSize, startDate, endDate);

            // Create a model compatible with your view
            var model = new TransactionInputModel
            {
                Transactions = data?.items ?? new List<TransactionModel>(),
                TotalCount = data?.totalCount ?? 0,
                PageNumber = page,
                PageSize = pageSize,
                StartDate = startDate,
                EndDate = endDate
            };

            return View("Pages/Transaction/Index.cshtml", model);
        }


    }
}
