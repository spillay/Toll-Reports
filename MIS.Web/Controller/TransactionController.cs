
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
        public IActionResult Index()
        {
            return View("Views/Index.cshtml");
        }
        public async Task<IActionResult> Transaction(int page = 1, int pageSize = 10)
        {
            // For now, we use static date range (you can later make this dynamic)
            var startDate = DateTime.Parse("08/08/2025");
            var endDate = DateTime.Parse("09/09/2025");

            // Create a model compatible with your view
            var model = new TransactionInputModel
            {
                page = page,
                pageSize = pageSize,
                StartDate = startDate,
                EndDate = endDate
            };


            // Fetch paginated data from your report service
            var data = await _reportService.GetTransactionDetailsAsync(model);          

            return View("Views/Transaction/Index.cshtml", data);
        }


    }
}
