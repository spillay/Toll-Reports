using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models;
using MIS.Web.Models.VarientPerfomance;
using MIS.Web.Services;

namespace MIS.Web.Controllers
{
    public class VarientPerfomanceController : Controller
    {
        private readonly IVarientPerfomanceReportService _reportService;

        public VarientPerfomanceController(IVarientPerfomanceReportService reportService)
        {
            _reportService = reportService;
        }

        
        public async Task<IActionResult> VarientPerfomances(int page = 1, int pageSize = 10)
        {
            // For now, we use static date range (you can later make this dynamic)
            var startDate = DateTime.Parse("08/08/2025");
            var endDate = DateTime.Parse("09/09/2025");

            // Fetch paginated data from your report service
            var data = await _reportService.GetVarientPerfomanceDetailsAsync(page, pageSize, startDate, endDate);

            // Create a model compatible with your view
            var model = new VarientPerfomanceInputModel
            {
                VarientPerfomances = data?.items ?? new List<VarientPerfomanceModel>(),
                TotalCount = data?.totalCount ?? 0,
                PageNumber = page,
                PageSize = pageSize,
                StartDate = startDate,
                EndDate = endDate
            };

            return View("Views/VarientPerfomance/Index.cshtml", model);
        }
    }
}
