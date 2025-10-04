using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MIS.Web.Models.Comprehensive;
using MIS.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MIS.Web.Pages
{
    public class ComprehensiveReportModel : PageModel
    {
        private readonly IComprehensiveReportService _reportService;

        public ComprehensiveReportModel(IComprehensiveReportService reportService)
        {
            _reportService = reportService;
        }

        // Filters
        [BindProperty(SupportsGet = true)]
        public DateTime StartDate { get; set; } = new DateTime(2025, 8, 8);

        [BindProperty(SupportsGet = true)]
        public DateTime EndDate { get; set; } = new DateTime(2025, 9, 9);

        [BindProperty(SupportsGet = true)]
        public string? Shift { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? methodOfPayment { get; set; }

        public List<ComprehensiveReportViewModel> comprehensives { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                // Fetch all records
                var data = await _reportService.GetComprehensiveDetailsAsync(StartDate, EndDate);

                // Apply filters
                if (!string.IsNullOrEmpty(Shift))
                    data = data.Where(t => t.operational_Shift == Shift).ToList();

                if (!string.IsNullOrEmpty(methodOfPayment))
                    data = data.Where(t => t.methodOfPayment == methodOfPayment).ToList();

                comprehensives = data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching report: " + ex);
                ErrorMessage = "Failed to load report data. Please try again later.";
            }
        }
    }
}
