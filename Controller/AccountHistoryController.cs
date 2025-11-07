using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MIS.Web.Models.AccountHistory;
using MIS.Web.Services;
using System;
using System.Threading.Tasks;

namespace MIS.Web.Controllers
{
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
        public async Task<IActionResult> Index(string? accountNumber = null)
        {
            var model = new AccountHistoryInputModel();

            try
            {
                // ✅ Load all available account numbers for dropdown
                var accounts = await _service.GetAccountsAsync();
                ViewBag.Accounts = accounts;

                // ✅ Load either specific account or all accounts
                if (!string.IsNullOrWhiteSpace(accountNumber))
                {
                    _logger.LogInformation("📄 Loading account history for {AccountNumber}", accountNumber);
                    model = await _service.GetAccountHistoryAsync(accountNumber);
                    model.AccountNumber = accountNumber; // preserve dropdown selection
                }
                else
                {
                    _logger.LogInformation("📊 Loading all account histories (no filter selected)");
                    model = await _service.GetAccountHistoryAsync(null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error loading account history");
                TempData["Error"] = "An error occurred while loading account history.";
            }

            return View(model);
        }
    }
}
