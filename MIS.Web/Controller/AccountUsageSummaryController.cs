using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models.AccountUsageSummary;
using MIS.Web.Services;
using System;
using System.Threading.Tasks;

namespace MIS.Web.Controllers
{
    [Authorize]
    public class AccountUsageSummaryController : Controller
    {
        private readonly IAccountUsageSummaryService _service;

        public AccountUsageSummaryController(IAccountUsageSummaryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index(AccountUsageSummaryInputModel filters)
        {
            var start = filters.StartDate ?? DateTime.Now.AddDays(-30);
            var end = filters.EndDate ?? DateTime.Now;

            var page = filters.Page <= 0 ? 1 : filters.Page;
            var pageSize = filters.PageSize <= 0 ? 20 : filters.PageSize;

            var model = await _service.GetAccountUsageSummaryAsync(
                start,
                end,
                filters.AccountNumber,
                page,
                pageSize
            );

            model.Filters = new AccountUsageSummaryInputModel
            {
                StartDate = start,
                EndDate = end,
                AccountNumber = filters.AccountNumber,
                Page = page,
                PageSize = pageSize
            };

            return View(model);
        }
    }
}