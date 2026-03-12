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

            // Multi-select checklist values come in as repeated query params:
            [FromQuery] List<int>? shiftIds,
            [FromQuery] List<long>? systemUserIds,

            int page = 1,
            int pageSize = 20)
        {
            //  Default date range (consistent with your reports)
            var start = (startDate ?? DateTime.Now.AddDays(-7));
            var end = (endDate ?? DateTime.Now);

            //  Ensure non-null lists
            shiftIds ??= new List<int>();
            systemUserIds ??= new List<long>();

            // 1) Get global filter options (NOT date filtered)
            var (shiftOptions, operatorOptions) = await _service.GetFiltersAsync();

            // 2) Mark selected options (so checkboxes stay checked)
            if (shiftOptions != null && shiftOptions.Count > 0 && shiftIds.Count > 0)
            {
                foreach (var opt in shiftOptions)
                    opt.Selected = shiftIds.Contains(opt.Id);
            }

            if (operatorOptions != null && operatorOptions.Count > 0 && systemUserIds.Count > 0)
            {
                foreach (var opt in operatorOptions)
                    opt.Selected = systemUserIds.Contains(opt.Id);
            }

            // 3) Fetch report data (ID-based filtering)
            var data = await _service.GetDailyCashupAsync(
                start,
                end,
                shiftIds,
                systemUserIds,
                page,
                pageSize
            );

            // 4) Attach filters + selections to the model (consistent)
            data.StartDate = start;
            data.EndDate = end;

            data.ShiftOptions = shiftOptions ?? new List<CheckItemModel<int>>();
            data.TollOperatorOptions = operatorOptions ?? new List<CheckItemModel<long>>();

            data.SelectedShiftIds = shiftIds;
            data.SelectedSystemUserIds = systemUserIds;

            return View(data);
        }
    }
}