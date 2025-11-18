using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models.AccountUsageDetails;
using MIS.Web.Services;
using System;
using System.Threading.Tasks;

namespace MIS.Web.Controllers
{
    public class AccountUsageDetailsController : Controller
    {
        private readonly IAccountUsageDetailsService _service;

        public AccountUsageDetailsController(IAccountUsageDetailsService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(AccountUsageDetailsInputModel filters)
        {
            // Default date range (last 30 days)
            DateTime start = filters.StartDate ?? DateTime.Now.AddDays(-30);
            DateTime end = filters.EndDate ?? DateTime.Now;

            // Call API (returns Summary + Details)
            var apiResponse = await _service.GetAccountUsageDetailsAsync(start, end);

            // Build Model for Razor Page
            var model = new PageAccountUsageDetailsModel
            {
                Summary = apiResponse?.Summary ?? new AccountUsageSummaryModel(),
                Items = apiResponse?.Items ?? new List<AccountUsageDetailsModel>()
            };

            // Send date values back to view
            ViewBag.StartDate = start;
            ViewBag.EndDate = end;

            return View(model);
        }
    }
}
