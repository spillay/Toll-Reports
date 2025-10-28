using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models.Discrepancy;
using MIS.Web.Services;
using System;
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

        public async Task<IActionResult> Index(
            DateTime? startDate = null,
            DateTime? endDate = null,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null,
            List<string>? laneNames = null,
            List<string>? paymentMethods = null,
            int page = 1,
            int pageSize = 50)
        {
            // Default date range
            var fromDate = startDate ?? DateTime.Today.AddDays(-7);
            var toDate = endDate ?? DateTime.Today;

            var model = await _service.GetDiscrepancyReportAsync(
                fromDate,
                toDate,
                operationalShift,
                tollOperators,
                laneNames,
                paymentMethods,
                takenAction: null, // or your List<string> of actions
                page,
                pageSize
            );


            return View(model);
        }
    }
}
