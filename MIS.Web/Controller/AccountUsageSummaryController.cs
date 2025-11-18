using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models.AccountUsageSummary;
using MIS.Web.Services;
using System;
using System.Threading.Tasks;

namespace MIS.Web.Controllers
{
    public class AccountUsageSummaryController : Controller
    {
        private readonly IAccountUsageSummaryService _service;

        public AccountUsageSummaryController(IAccountUsageSummaryService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            DateTime start = startDate ?? DateTime.Now.AddDays(-30);
            DateTime end = endDate ?? DateTime.Now;

            var model = await _service.GetAccountUsageSummaryAsync(start, end);

            ViewBag.StartDate = start;
            ViewBag.EndDate = end;

            return View(model);
        }
    }
}
