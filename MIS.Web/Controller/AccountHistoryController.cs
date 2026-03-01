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
                ViewBag.Accounts = await _service.GetAccountsAsync();

                // ============================================================
                // RULE: Account filter ONLY works when operational = true
                // ============================================================
                bool useAccountFilter = operational == true && !string.IsNullOrWhiteSpace(accountNumber);

                if (!useAccountFilter)
                    accountNumber = null;  // Force API to return ALL ACCOUNTS

                // If using operational mode, date range is required
                if (operational == true && (startDate == null || endDate == null))
                {
                    TempData["Warning"] = "Start Date and End Date are required in operational mode.";
                    return View(model);
                }

                // Fetch data
                model = await _service.GetAccountHistoryAsync(
                    accountNumber,
                    startDate,
                    endDate,
                    operational);

                // Push filter values back to UI
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
