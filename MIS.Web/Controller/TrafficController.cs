using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models.Traffic;
using MIS.Web.Services;

namespace MIS.Web.Controllers
{
    public class TrafficController : Controller
    {
        private readonly ITrafficReportService _trafficReportService;
        private readonly ILogger<TrafficController> _logger;

        public TrafficController(ITrafficReportService trafficReportService, ILogger<TrafficController> logger)
        {
            _trafficReportService = trafficReportService;
            _logger = logger;
        }

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> Traffic([FromQuery] TrafficInputModel input, int pageNumber = 1, int pageSize = 10)
        {
            if (input.StartDate == default) input.StartDate = DateTime.UtcNow.Date;
            if (input.EndDate == default) input.EndDate = DateTime.UtcNow.Date.AddDays(1).AddSeconds(-1);

            List<string>? classificationList = null;
            if (!string.IsNullOrWhiteSpace(input.Classification))
                classificationList = input.Classification.Split(',').Select(s => s.Trim()).ToList();

            var pageModel = await _trafficReportService.GetTrafficReportAsync(pageNumber, pageSize, input.StartDate, input.EndDate, classificationList);

            ViewBag.InputModel = input;
            ViewBag.Classifications = new List<string> { "Class 1", "Class 2", "Class 4", "Class M" };

            return View("Index", pageModel);
        }


        [HttpGet]
        public async Task<IActionResult> GetAllTrafficJson(
            [FromQuery] TrafficInputModel input)
        {
            try
            {
                if (input.StartDate == default) input.StartDate = DateTime.UtcNow.Date;
                if (input.EndDate == default) input.EndDate = DateTime.UtcNow.Date.AddDays(1).AddSeconds(-1);

                List<string>? classificationList = null;
                if (!string.IsNullOrWhiteSpace(input.Classification))
                    classificationList = input.Classification.Split(',').Select(s => s.Trim()).ToList();

                var pageModel = await _trafficReportService.GetTrafficReportAsync(
                    1, int.MaxValue / 1000, input.StartDate, input.EndDate, classificationList);

                var data = pageModel?.Items?.Cast<object>().ToList() ?? new List<object>();

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all traffic for export");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
