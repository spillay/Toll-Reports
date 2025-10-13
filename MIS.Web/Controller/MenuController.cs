
using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models;
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
        public async Task<IActionResult> Transaction()

        {
           var data= await _reportService.GetTransactionDetailsAsync(1, 10, DateTime.Parse("08/08/2025"), DateTime.Parse("09/09/2025"));
            var model = new TransactionInputModel();
            return View("Pages/Transaction/Index.cshtml",model);
        }
       
    }
}
