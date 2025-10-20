using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models;
using MIS.Web.Models.VarientPerfomance;
using MIS.Web.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MIS.Web.Controllers
{
    public class VarientPerfomanceController : Controller
    {
        private readonly IVarientPerfomanceReportService _reportService;

        public VarientPerfomanceController(IVarientPerfomanceReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<IActionResult> VarientPerfomances(
            int page = 1, int pageSize = 10,
            string? shift = null, string? tollOperatorID = null,
            DateTime? startDate = null, DateTime? endDate = null)
        {
            
            startDate ??= DateTime.Today.AddDays(-90);
            endDate ??= DateTime.Today;

            
            var shifts = string.IsNullOrEmpty(shift) ? null : new List<string> { shift };
            var operators = string.IsNullOrEmpty(tollOperatorID) ? null : new List<string> { tollOperatorID };

           
            var data = await _reportService.GetVarientPerfomanceDetailsAsync(
                page, pageSize, startDate.Value, endDate.Value, shifts, operators);

            
            ViewBag.Shifts = new List<string> { "Shift One", "Shift Two", "Shift Three" };
            ViewBag.TollOperators = new List<string> { "0001", "0002", "0003", "0004", "0005" };

            
            int totalPages = 0;
            if (data.totalCount > 0)
            {
                totalPages = (int)Math.Ceiling((double)data.totalCount / pageSize);
            }

            
            var model = new VarientPerfomanceInputModel
            {
                items = data.items ?? new List<VarientPerfomanceModel>(),
                totalCount = data.totalCount,
                page = page,
                pageSize = pageSize,
                totalPages = totalPages,
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                Shift = shift,
                TollOperatorID = tollOperatorID
            };

            return View("Views/VarientPerfomance/Index.cshtml", model);
        }
    }
}
