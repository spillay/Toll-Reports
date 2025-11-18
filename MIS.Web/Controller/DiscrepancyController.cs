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
        private readonly IConfiguration _configuration;

        public DiscrepancyController(IDiscrepancyReportService service, IConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index([FromQuery] PageDiscrepancyModel model)
        {
            // -------------------------------
            // 1. ENSURE FILTER MODEL EXISTS
            // -------------------------------
            model ??= new PageDiscrepancyModel();
            model.Filters ??= new DiscrepancyInputModel();

            if (model.Filters.StartDate == default)
                model.Filters.StartDate = DateTime.Today.AddDays(-7);

            if (model.Filters.EndDate == default)
                model.Filters.EndDate = DateTime.Today;

            model.page = model.page <= 0 ? 1 : model.page;
            model.pageSize = model.pageSize <= 0 ? 50 : model.pageSize;

            // Helper: convert string to list<string>
            static List<string>? ToList(string? v)
                => string.IsNullOrWhiteSpace(v) ? null : new List<string> { v };

            // -------------------------------
            // 2. API CALL (MAIN REQUEST)
            // -------------------------------
            var data = await _service.GetDiscrepancyReportAsync(
                model.Filters.StartDate,
                model.Filters.EndDate,
                ToList(model.Filters.Shift),
                ToList(model.Filters.toll_Operator_ID),
                ToList(model.Filters.lane_Nr),
                ToList(model.Filters.PaymentMethod),
                ToList(model.Filters.TakenAction),
                model.page,
                model.pageSize
            );

            data ??= new PageDiscrepancyModel();
            data.Items ??= new List<DiscrepancyModel>();

            // -------------------------------
            // 3. BUILD DROPDOWNS FROM RESULTS
            // -------------------------------
            var source = data.Items;

            ViewBag.TollOperators = source
                .Select(x => x.Toll_Operator_ID)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct().OrderBy(x => x).ToList();

            ViewBag.Shifts = source
                .Select(x => x.Operational_Shift)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct().OrderBy(x => x).ToList();

            ViewBag.PaymentMethods = source
                .Select(x => x.Method_of_Payment)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct().OrderBy(x => x).ToList();

            ViewBag.Lanes = source
                .Select(x => x.Lane_Nr.ToString())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct().OrderBy(x => x).ToList();

            ViewBag.TakenActions = source
                .Select(x => x.TakenAction)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct().OrderBy(x => x).ToList();

            // -------------------------------
            // 4. PRESERVE FILTERS
            // -------------------------------
            data.Filters = model.Filters;
            data.page = model.page;
            data.pageSize = model.pageSize;

            // -------------------------------
            // 5. PROVIDE EXPORT API ENDPOINT
            // -------------------------------
            ViewData["DiscrepancyApi"] =
                $"{_configuration["BaseApiUrl:Link"]}{_configuration["ApiSettings:DiscrepancyReportEndpoint"]}";

            // -------------------------------
            // 6. RETURN VIEW
            // -------------------------------
            return View("~/Views/Discrepancy/Index.cshtml", data);
        }
    }
}
