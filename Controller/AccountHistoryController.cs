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

        public AccountHistoryController(
            IAccountHistoryService service,
            ILogger<AccountHistoryController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
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
                accountNumber = (accountNumber ?? string.Empty).Trim();

                // Account must be selected first
                if (string.IsNullOrWhiteSpace(accountNumber))
                {
                    model.AccountNumber = accountNumber;
                    model.StartDate = startDate;
                    model.EndDate = endDate;
                    model.Operational = operational;

                    TempData["Warning"] = "Please select an Account Number first.";
                    return View(model);
                }

                // If one date is provided, both are required
                if ((startDate.HasValue && !endDate.HasValue) ||
                    (!startDate.HasValue && endDate.HasValue))
                {
                    model.AccountNumber = accountNumber;
                    model.StartDate = startDate;
                    model.EndDate = endDate;
                    model.Operational = operational;

                    TempData["Warning"] = "Please provide BOTH Start Date and End Date, or leave both empty.";
                    return View(model);
                }

                model = await _service.GetAccountHistoryAsync(
                    accountNumber,
                    startDate,
                    endDate,
                    operational
                );

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