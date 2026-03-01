using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models.Traffic.Hourly;
using MIS.Web.Services;

namespace MIS.Web.Controllers
{
    [Authorize]
    public class TrafficController : Controller
    {
        private readonly IHourlyTrafficReportService _trafficService;

        public TrafficController(IHourlyTrafficReportService trafficService)
        {
            _trafficService = trafficService;
        }

        [HttpGet]
        public async Task<IActionResult> HourlyReport(
     DateTime? startDate = null,
     DateTime? endDate = null,
     string? classification = null,
     List<int>? shifts = null,
     bool operationalDay = false)
        {
            DateTime start = startDate ?? DateTime.Today;
            DateTime end = endDate ?? DateTime.Today;

            List<string>? selectedClasses = string.IsNullOrWhiteSpace(classification)
                ? null
                : classification.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(c => c.Trim())
                                .Where(c => !string.IsNullOrWhiteSpace(c))
                                .Distinct(StringComparer.OrdinalIgnoreCase)
                                .ToList();

            // ✅ DB classes for checklist + table columns
            var allClasses = await _trafficService.GetAllClassificationsAsync();

            var trafficItems = await _trafficService.GetTrafficReportAsync(
                start, end, selectedClasses, shifts, operationalDay
            );
            var allClasse = await _trafficService.GetAllClassificationsAsync();
            Console.WriteLine("Classes from API: " + string.Join(", ", allClasse ?? new List<string>()));
            var model = new PageHourlyTrafficModel
            {
                Items = trafficItems.Items,
                Classifications = allClasses ?? new List<string>(),   // ✅ THIS IS THE KEY LINE
                Input = new HourlyTrafficInputModel
                {
                    StartDate = start,
                    EndDate = end,
                    Classification = classification,
                    Shifts = shifts ?? new List<int>(),
                    OperationalDay = operationalDay
                }
            };

            return View("~/Views/Traffic/Hourly/Index.cshtml", model);
        }


    }
}
