using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models.DailyCashup;
using MIS.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MIS.Web.Controllers
{
    [Authorize]
    public class DailyCashupController : Controller
    {
        private readonly IDailyCashupReportService _service;

        public DailyCashupController(IDailyCashupReportService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            DateTime? startDate,
            DateTime? endDate,
            [FromQuery] List<int>? shiftIds,
            [FromQuery] List<long>? systemUserIds,
            int page = 1,
            int pageSize = 20)
        {
            var start = (startDate ?? DateTime.Today.AddDays(-7)).Date;
            var end = (endDate ?? DateTime.Today).Date;

            shiftIds ??= new List<int>();
            systemUserIds ??= new List<long>();

            // 1) Load filter options
            var (shiftOptions, operatorOptions) = await _service.GetFiltersAsync();

            // 2) Load report data
            var model = await _service.GetDailyCashupAsync(
                start,
                end,
                shiftIds,
                systemUserIds,
                page,
                pageSize);

            // 3) Attach filter options
            model.ShiftOptions = shiftOptions ?? new List<CheckItemModel<int>>();
            model.TollOperatorOptions = operatorOptions ?? new List<CheckItemModel<long>>();

            // 4) Persist selected filters
            model.SelectedShiftIds = shiftIds;
            model.SelectedSystemUserIds = systemUserIds;
            model.StartDate = start;
            model.EndDate = end;

            // 5) Mark selected options
            foreach (var opt in model.ShiftOptions)
            {
                opt.Selected = shiftIds.Contains(opt.Id);
            }

            foreach (var opt in model.TollOperatorOptions)
            {
                opt.Selected = systemUserIds.Contains(opt.Id);
            }

            return View(model);
        }
    }
}