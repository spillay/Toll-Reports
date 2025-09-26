using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MIS.Web.Services;

namespace MIS.Web.Controller
{
    public class TransactionController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IReportService reportService;
        public TransactionController(IReportService _reportService)
        {
            reportService = _reportService;
        }
        public IActionResult Index()
        {
            var data = reportService.GetShiftsAsync().GetAwaiter().GetResult();
            return View();
        }
    }
}
