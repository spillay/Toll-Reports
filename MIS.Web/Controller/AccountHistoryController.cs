using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MIS.Web.Models.AccountHistory;
using MIS.Web.Services;
using System;
using System.Threading.Tasks;

namespace MIS.Web.Controllers
{
    [Authorize]
    public class AccountHistoryController : Controller
    {
        private readonly IAccountHistoryService _service;
        private readonly ILogger<AccountHistoryController> _logger;

        public AccountHistoryController(IAccountHistoryService service, ILogger<AccountHistoryController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchAccounts(string q, int take = 20)
        {
            var results = await _service.SearchAccountsAsync(q, take);
            return Json(results);
        }
        [HttpGet]
        public async Task<IActionResult> Index(
            string? accountNumber = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            bool? operational = null)
        {
            var model = new AccountHistoryInputModel();

            try
            {
              
                accountNumber = (accountNumber ?? "").Trim();

                //  Account first
                if (string.IsNullOrWhiteSpace(accountNumber))
                {
                    model.Operational = operational;
                    model.StartDate = startDate;
                    model.EndDate = endDate;

                    TempData["Warning"] = "Please select an Account Number first.";
                    return View(model);
                }

                // ✅ If user sets one date, require both
                if ((startDate.HasValue && !endDate.HasValue) || (!startDate.HasValue && endDate.HasValue))
                {
                    TempData["Warning"] = "Please provide BOTH Start Date and End Date, or leave both empty.";
                    model.AccountNumber = accountNumber;
                    model.StartDate = startDate;
                    model.EndDate = endDate;
                    model.Operational = operational;
                    return View(model);
                }

                // Fetch from API
                model = await _service.GetAccountHistoryAsync(
                    accountNumber,
                    startDate,
                    endDate,
                    operational
                );

                // Push filters back to UI (service already does some, but keep consistent)
                model.AccountNumber = accountNumber;
                model.StartDate = startDate;
                model.EndDate = endDate;
                model.Operational = operational;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading account history");
                TempData["Error"] = "Unexpected error occurred.";
            }

            return View(model);
        }
    }
}