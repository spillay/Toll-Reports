using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models;
using MIS.Web.Services;
using System;
using System.Threading.Tasks;

namespace MIS.Web.Controllers
{
    [Authorize]
    public class TopUpController : Controller
    {
        private readonly ITopUpReportService _service;

        public TopUpController(ITopUpReportService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index(TopUpInputModel model)
        {
            // 1) Default dates
            var start = model.StartDate ?? DateTime.Now.AddDays(-1);
            var end = model.EndDate ?? DateTime.Now;

            // 2) Operational day range logic
            if (model.OperationalDate == true)
            {
                start = start.Date.AddHours(5.5);                         // 05:30
                end = end.Date.AddDays(1).AddHours(5.5).AddSeconds(-1);   // Next day 05:29:59
            }

            // 3) Load GLOBAL filter options (master tables)
            var options = await _service.GetTopUpFilterOptionsAsync();

            model.ShiftOptions = options.ShiftOptions ?? model.ShiftOptions;
            model.OperatorOptions = options.OperatorOptions ?? model.OperatorOptions;
            model.LaneOptions = options.LaneOptions ?? model.LaneOptions;
            model.PaymentMethodOptions = options.PaymentMethodOptions ?? model.PaymentMethodOptions;

            // 4) Fetch paged data using checkbox lists
            var result = await _service.GetTopUpAsync(
                start,
                end,
                model.Shifts,
                model.OperatorIds,
                model.Lanes,
                model.PaymentMethods,
                model.AccountNumber,
                model.page,
                model.pageSize
            );

            model.PageData = result;
            model.StartDate = start;
            model.EndDate = end;

            return View(model);
        }

        [HttpGet("TopUp/export")]
        public async Task<IActionResult> Export(TopUpInputModel model)
        {
            var start = model.StartDate ?? DateTime.Now.AddDays(-1);
            var end = model.EndDate ?? DateTime.Now;

            if (model.OperationalDate == true)
            {
                start = start.Date.AddHours(5.5);
                end = end.Date.AddDays(1).AddHours(5.5).AddSeconds(-1);
            }

            var allData = await _service.GetTopUpFullAsync(
                start,
                end,
                model.Shifts,
                model.OperatorIds,
                model.Lanes,
                model.PaymentMethods,
                model.AccountNumber
            );

            return Json(allData.Items ?? new System.Collections.Generic.List<MIS.Web.Models.TopUp.TopUpModel>());
        }
    }
}