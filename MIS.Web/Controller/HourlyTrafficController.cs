using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models.Traffic.Hourly;
using MIS.Web.Services;

namespace MIS.Web.Controllers
{
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

            List<string>? classifications = string.IsNullOrEmpty(classification)
                ? null
                : classification.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(c => c.Trim())
                                .ToList();

            var trafficItems = await _trafficService.GetTrafficReportAsync(
                start,
                end,
                classifications,
                shifts,
                operationalDay
            );

            // Wrap data into PageHourlyTrafficModel
            var model = new PageHourlyTrafficModel
            {
                Items = trafficItems.Items,
                Input = new HourlyTrafficInputModel
                {
                    StartDate = start,
                    EndDate = end,
                    Classification = classification,
                    Shifts = shifts ?? new List<int>(),
                    OperationalDay = operationalDay
                }
            };

            ViewBag.Classifications = new List<string> { "Class 1", "Class 2", "Class 4", "Class M" };
            Console.WriteLine("Shifts: " + (shifts != null ? string.Join(",", shifts) : "none"));
            Console.WriteLine("OperationalDay: " + operationalDay);

            return View("~/Views/Traffic/Hourly/Index.cshtml", model);
        }

    }
}
