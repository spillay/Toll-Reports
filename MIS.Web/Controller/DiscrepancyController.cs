using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MIS.Web.Models.Discrepancy;
using MIS.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MIS.Web.Controllers
{
    [Authorize]
    public class DiscrepancyController : Controller
    {
        private readonly IDiscrepancyReportService _service;
        private readonly IConfiguration _configuration;

        public DiscrepancyController(
            IDiscrepancyReportService service,
            IConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] PageDiscrepancyModel model)
        {
            try
            {
                /* ======================================
                 * 1) INIT + DEFAULTS
                 * ====================================== */
                model ??= new PageDiscrepancyModel();
                model.Filters ??= new DiscrepancyInputModel();

                // Defaults (same pattern as Transaction)
                if (model.Filters.StartDate == default)
                    model.Filters.StartDate = DateTime.Today.AddDays(-7).Date.AddHours(0);

                if (model.Filters.EndDate == default)
                    model.Filters.EndDate = DateTime.Today.Date.AddDays(1).AddSeconds(-1);

                // Safety: prevent inverted dates
                if (model.Filters.EndDate < model.Filters.StartDate)
                    model.Filters.EndDate = model.Filters.StartDate.AddHours(1);

                // Paging (your PageDiscrepancyModel uses page/pageSize)
                model.page = model.page <= 0 ? 1 : model.page;
                model.pageSize = model.pageSize <= 0 ? 50 : model.pageSize;

                // Mirror paging into Filters if you use Filters.Page/PageSize in service
                model.Filters.Page = model.page;
                model.Filters.PageSize = model.pageSize;

                /* ======================================
                 * 2) NORMALIZE CHECKLIST VALUES
                 * (Trim, remove empties, distinct)
                 * ====================================== */
                model.Filters.SelectedShifts = Normalize(model.Filters.SelectedShifts);
                model.Filters.SelectedTollOperators = Normalize(model.Filters.SelectedTollOperators);
                model.Filters.SelectedLanes = Normalize(model.Filters.SelectedLanes);
                model.Filters.SelectedPaymentMethods = Normalize(model.Filters.SelectedPaymentMethods);
                model.Filters.SelectedTakenActions = Normalize(model.Filters.SelectedTakenActions);

                /* ======================================
                 * 3) LOAD FILTER OPTIONS (ALL VALUES FROM DB)
                 * ====================================== */
                var options = await _service.GetDiscrepancyFilterOptionsAsync(model.Filters);

                // Update the option lists (keep user selections)
                model.Filters.Shifts = options.Shifts ?? new List<string>();
                model.Filters.TollOperators = options.TollOperators ?? new List<string>();
                model.Filters.Lanes = options.Lanes ?? new List<string>();
                model.Filters.PaymentMethods = options.PaymentMethods ?? new List<string>();
                model.Filters.TakenActions = options.TakenActions ?? new List<string>();

                /* ======================================
                 * 4) LOAD PAGINATED GRID DATA
                 * ====================================== */
                var gridData = await _service.GetDiscrepancyReportAsync(model.Filters);

                gridData ??= new PageDiscrepancyModel();
                gridData.Items ??= new List<DiscrepancyModel>();

                // Copy paging metadata back
                model.Items = gridData.Items;
                model.totalCount = gridData.totalCount;
                model.totalPages = gridData.totalPages;
                model.page = gridData.page;         // in case API adjusted
                model.pageSize = gridData.pageSize; // in case API adjusted

                // Keep filters (with option lists + selections)
                model.Filters = model.Filters; // already set

                /* ======================================
                 * 5) LOAD FULL EXPORT DATA (exportAll=true)
                 * ====================================== */
                var exportData = await _service.GetFullExportAsync(model.Filters);

                model.ExportItems = exportData?.ExportItems ?? exportData?.Items ?? new List<DiscrepancyModel>();

                /* ======================================
                 * 6) EXPORT API URL (optional - kept)
                 * ====================================== */
                ViewData["DiscrepancyApi"] =
                    $"{_configuration["BaseApiUrl:Link"]}{_configuration["ApiSettings:DiscrepancyReportEndpoint"]}";

                /* ======================================
                 * 7) RETURN VIEW
                 * ====================================== */
                return View("~/Views/Discrepancy/Index.cshtml", model);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ ERROR in DiscrepancyController: " + ex.Message);

                return View("~/Views/Discrepancy/Index.cshtml", new PageDiscrepancyModel
                {
                    Items = new List<DiscrepancyModel>(),
                    ExportItems = new List<DiscrepancyModel>(),
                    Filters = new DiscrepancyInputModel()
                });
            }
        }

        // ✅ helper: normalize checklist inputs
        private static List<string> Normalize(List<string>? list)
        {
            return (list ?? new List<string>())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }
}