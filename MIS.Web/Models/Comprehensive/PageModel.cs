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
        public string? TransactionType { get; set; }

        public List<ComprehensiveReportViewModel> comprehensives { get; set; } = new();

        public List<string> TollClasses { get; set; } = new(); // e.g., Class 1–4
        public List<dynamic> GroupedData { get; set; } = new();

        public async Task OnGetAsync()
        {
            var data = await _reportService.GetComprehensiveDetailsAsync(StartDate, EndDate);

            // Apply filters
            if (!string.IsNullOrEmpty(Shift))
                data = data.Where(t => t.Shift == Shift).ToList();

            if (!string.IsNullOrEmpty(TransactionType))
                data = data.Where(t => t.TransactionType == TransactionType).ToList();

            comprehensives = data;

            // Get distinct toll classes dynamically
            TollClasses = data
                .Select(d => d.ManualTollClass)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            // Group by Method of Payment (TransactionType)
            GroupedData = data
                .GroupBy(t => t.TransactionType ?? "Unknown")
                .Select(g =>
                {
                    var totalCount = g.Count();
                    var totalRevenue = g.Sum(x => x.AmountInclusive);

                    // Generate dynamic dictionary for classes
                    var classData = TollClasses.ToDictionary(
                        c => c,
                        c => new
                        {
                            Count = g.Count(x => x.ManualTollClass == c),
                            CountPercent = totalCount == 0 ? 0 : (decimal)g.Count(x => x.ManualTollClass == c) / totalCount * 100,
                            Revenue = g.Where(x => x.ManualTollClass == c).Sum(x => x.AmountInclusive),
                            RevenuePercent = totalRevenue == 0 ? 0 : (decimal)(g.Where(x => x.ManualTollClass == c).Sum(x => x.AmountInclusive) / totalRevenue * 100)
                        });

                    return new
                    {
                        Method = g.Key,
                        Classes = classData,
                        TotalCount = totalCount,
                        TotalRevenue = totalRevenue
                    };
                })
                .ToList<dynamic>();
        }
    }
}
