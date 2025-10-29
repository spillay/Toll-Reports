using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models.Discrepancy;
using MIS.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MIS.Web.Controllers
{
    public class DiscrepancyController : Controller
    {
        private readonly IDiscrepancyReportService _service;

        public DiscrepancyController(IDiscrepancyReportService service)
        {
            _service = service;
        }

        // Bind from query so Filters.* can arrive via query string
        public async Task<IActionResult> Index([FromQuery] PageDiscrepancyModel model)
        {
            model ??= new PageDiscrepancyModel();
            model.Filters ??= new DiscrepancyInputModel();

            if (model.Filters.StartDate == default)
                model.Filters.StartDate = DateTime.Today.AddDays(-7);
            if (model.Filters.EndDate == default)
                model.Filters.EndDate = DateTime.Today;

            model.page = model.page <= 0 ? 1 : model.page;
            model.pageSize = model.pageSize <= 0 ? 50 : model.pageSize;

            static List<string>? ToListOrNull(string? v)
                => string.IsNullOrWhiteSpace(v) ? null : new List<string> { v };

            // Map form single-selects into list query params for API
            var operationalShift = ToListOrNull(model.Filters.Shift);
            var tollOperators = ToListOrNull(model.Filters.toll_Operator_ID);
            var laneNames = ToListOrNull(model.Filters.lane_Nr);
            var paymentMethods = ToListOrNull(model.Filters.PaymentMethod);
            var takenActions = ToListOrNull(model.Filters.TakenAction);

            var data = await _service.GetDiscrepancyReportAsync(
                model.Filters.StartDate,
                model.Filters.EndDate,
                operationalShift,
                tollOperators,
                laneNames,
                paymentMethods,
                takenActions,
                model.page,
                model.pageSize
            );

            var allData = await _service.GetDiscrepancyReportAsync(
                model.Filters.StartDate,
                model.Filters.EndDate,
                null, null, null, null, null,
                page: 1,
                pageSize: int.MaxValue
            );

            ViewBag.TollOperators = allData.Items?
                .Select(t => t.toll_Operator_ID)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList() ?? new List<string>();

            ViewBag.Shifts = allData.Items?
                .Select(t => t.operational_Shift)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList() ?? new List<string>();

            ViewBag.PaymentMethods = allData.Items?
                .Select(t => t.method_of_Payment)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList() ?? new List<string>();

            ViewBag.TakenActions = allData.Items?
                .Select(t => t.takenAction)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList() ?? new List<string>();

            data.Filters ??= new DiscrepancyInputModel();
            data.Filters.StartDate = model.Filters.StartDate;
            data.Filters.EndDate = model.Filters.EndDate;
            data.Filters.lane_Nr = model.Filters.lane_Nr;
            data.Filters.Shift = model.Filters.Shift;
            data.Filters.PaymentMethod = model.Filters.PaymentMethod;
            data.Filters.TakenAction = model.Filters.TakenAction;
            data.Filters.toll_Operator_ID = model.Filters.toll_Operator_ID;

            data.page = model.page;
            data.pageSize = model.pageSize;

            
            return View("Views/Discrepancy/Index.cshtml", data);
        }
    }
}
