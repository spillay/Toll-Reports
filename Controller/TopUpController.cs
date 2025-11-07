using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models;
using MIS.Web.Models.TopUp;
using MIS.Web.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MIS.Web.Controllers
{
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
            var start = model.StartDate ?? DateTime.Now.AddDays(-1);
            var end = model.EndDate ?? DateTime.Now;

            if (model.OperationalDate == true)
            {
                start = start.Date.AddHours(5.5);
                end = end.Date.AddDays(1).AddHours(5.5).AddSeconds(-1);
            }

            var result = await _service.GetTopUpAsync(
                start,
                end,
                model.Operator,
                model.Lane,
                model.Shift,
                model.AccountNumber,
                model.OperationalDate,
                model.page,
                model.pageSize
            );

            ViewBag.Shifts = result.items?.Select(i => i.Shift).Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList() ?? new List<string>();
            ViewBag.Lanes = result.items?.Select(i => i.LaneWorkstation).Where(l => !string.IsNullOrEmpty(l)).Distinct().ToList() ?? new List<string>();
            ViewBag.AccountNumbers = result.items?.Select(i => i.AccountNumber).Where(a => !string.IsNullOrEmpty(a)).Distinct().ToList() ?? new List<string>();

            model.PageData = result;
            return View(model);
        }
    }
}
