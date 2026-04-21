using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models;
using MIS.Web.Models.VarientPerfomance;
using MIS.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MIS.Web.Controllers
{
    [Authorize]
    public class VarientPerfomanceController : Controller
    {
        private readonly IVarientPerfomanceReportService _reportService;

        public VarientPerfomanceController(IVarientPerfomanceReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        public async Task<IActionResult> ExportVarientPerfomanceData(
            DateTime? startDate = null,
            DateTime? endDate = null,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null)
        {
            startDate ??= DateTime.Today.AddDays(-90);
            endDate ??= DateTime.Today;

            operationalShift ??= new List<string>();
            tollOperators ??= new List<string>();

            var exportData = await _reportService.GetVarientPerfomanceDetailsAsync(
                1,
                int.MaxValue,
                startDate.Value,
                endDate.Value,
                operationalShift.Any() ? operationalShift : null,
                tollOperators.Any() ? tollOperators : null);

            return Json(exportData.items ?? new List<VarientPerfomanceModel>());
        }

        [HttpGet]
        public async Task<IActionResult> VarientPerfomances(
            int page = 1,
            int pageSize = 10,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            startDate ??= DateTime.Today.AddDays(-90);
            endDate ??= DateTime.Today;

            operationalShift ??= new List<string>();
            tollOperators ??= new List<string>();

            if (page <= 0)
                page = 1;

            if (pageSize <= 0)
                pageSize = 10;

            ViewBag.AllShifts = await _reportService.GetAllShiftsAsync();
            ViewBag.AllOperators = await _reportService.GetAllTollOperatorsAsync();

            ViewBag.SelectedShifts = operationalShift;
            ViewBag.SelectedOperators = tollOperators;
            ViewBag.StartDate = startDate.Value;
            ViewBag.EndDate = endDate.Value;

            var pagedData = await _reportService.GetVarientPerfomanceDetailsAsync(
                page,
                pageSize,
                startDate.Value,
                endDate.Value,
                operationalShift.Any() ? operationalShift : null,
                tollOperators.Any() ? tollOperators : null);

            var exportData = await _reportService.GetVarientPerfomanceDetailsAsync(
                1,
                int.MaxValue,
                startDate.Value,
                endDate.Value,
                operationalShift.Any() ? operationalShift : null,
                tollOperators.Any() ? tollOperators : null);

            int totalPages = pagedData.totalCount > 0
                ? (int)Math.Ceiling((double)pagedData.totalCount / pageSize)
                : 0;

            var model = new VarientPerfomanceInputModel
            {
                items = pagedData.items ?? new List<VarientPerfomanceModel>(),
                totalCount = pagedData.totalCount,
                page = page,
                pageSize = pageSize,
                totalPages = totalPages,
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                OperationalShift = operationalShift,
                TollOperators = tollOperators,
                ExportItems = exportData.items ?? new List<VarientPerfomanceModel>()
            };

            return View("~/Views/VarientPerfomance/Index.cshtml", model);
        }
    }
}
