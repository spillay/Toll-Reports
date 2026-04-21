using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models.AccountUsageDetails;
using MIS.Web.Services;
using System;
using System.Threading.Tasks;

namespace MIS.Web.Controllers
{
    [Authorize]
    public class AccountUsageDetailsController : Controller
    {
        private readonly IAccountUsageDetailsService _service;

        public AccountUsageDetailsController(IAccountUsageDetailsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> SearchAccounts(string q, int take = 20)
        {
            var results = await _service.SearchAccountsAsync(q, take);
            return Json(results);
        }

        [HttpGet]
        public async Task<IActionResult> Index(AccountUsageDetailsInputModel filters)
        {
            var accountNumber = (filters.AccountNumber ?? string.Empty).Trim();
            var start = filters.StartDate ?? DateTime.Now.AddDays(-30);
            var end = filters.EndDate ?? DateTime.Now;

            ViewBag.AccountNumber = accountNumber;
            ViewBag.StartDate = start;
            ViewBag.EndDate = end;

            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                return View(new PageAccountUsageDetailsModel
                {
                    Header = new AccountUsageDetailsHeaderModel(),
                    Items = new()
                });
            }

            var model = await _service.GetAccountUsageDetailsAsync(accountNumber, start, end)
                        ?? new PageAccountUsageDetailsModel();

            model.Header ??= new AccountUsageDetailsHeaderModel();
            model.Items ??= new();

            return View(model);
        }
    }
}