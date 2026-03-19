using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIS.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MIS.Web.Controllers
{
    [Authorize]
    public class AvcAccuracyReportController : Controller
    {
        private readonly IAvcAccuracyReportService _service;

        public AvcAccuracyReportController(IAvcAccuracyReportService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            DateTime? startDate = null,
            DateTime? endDate = null,
            List<int>? shiftIds = null,
            List<int>? laneIds = null,
            List<int>? classIds = null)
        {
            var selectedDate = startDate?.Date ?? DateTime.Today;

            var finalStartDate = startDate ?? selectedDate.AddHours(5).AddMinutes(30);
            var finalEndDate = endDate ?? selectedDate.AddDays(1).AddHours(5).AddMinutes(29);

            var model = await _service.GetReportAsync(
                finalStartDate,
                finalEndDate,
                shiftIds ?? new List<int>(),
                laneIds ?? new List<int>(),
                classIds ?? new List<int>());

            return View(model);
        }
    }
}