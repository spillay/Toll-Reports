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
        public DateTime StartDate { get; set; } = DateTime.UtcNow.AddDays(-7);

        [BindProperty(SupportsGet = true)]
        public DateTime EndDate { get; set; } = DateTime.UtcNow;

        [BindProperty(SupportsGet = true)]
        public string? Shift { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? TransactionType { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? TollOperatorID { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? LaneName { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? MethodOfPayment { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? DiscountType { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Classification { get; set; }

        // NEW: GroupBy
        [BindProperty(SupportsGet = true)]
        public string GroupBy { get; set; } = "TransactionType";

        public List<ComprehensiveReportViewModel> comprehensives { get; set; } = new();

        // Dropdown data
        public List<string> TollClasses { get; set; } = new();
        public List<string> Shifts { get; set; } = new();
        public List<string> TransactionTypes { get; set; } = new();
        public List<string> TollOperators { get; set; } = new();
        public List<string> Lanes { get; set; } = new();
        public List<string> PaymentMethods { get; set; } = new();
        public List<string> DiscountTypes { get; set; } = new();
        public List<string> Classifications { get; set; } = new();

        public List<dynamic> GroupedData { get; set; } = new();

        public async Task OnGetAsync()
        {
            var data = await _reportService.GetComprehensiveDetailsAsync(StartDate, EndDate);

            // === Apply filters ===
            if (!string.IsNullOrEmpty(Shift))
                data = data.Where(t => t.Shift.Trim() == Shift).ToList();

            if (!string.IsNullOrEmpty(TransactionType))
                data = data.Where(t => t.TransactionType.Trim() == TransactionType).ToList();

            //if (!string.IsNullOrEmpty(TollOperatorID))
            //    data = data.Where(t => t.TollOperatorID == TollOperatorID).ToList();

            if (!string.IsNullOrEmpty(LaneName))
                data = data.Where(t => t.LaneName == LaneName).ToList();

            if (!string.IsNullOrEmpty(MethodOfPayment))
                data = data.Where(t => t.TransactionType.Trim() == MethodOfPayment).ToList();

            if (!string.IsNullOrEmpty(DiscountType))
                data = data.Where(dt => dt.DiscountType == DiscountType).ToList();

            if (!string.IsNullOrEmpty(Classification))
                data = data.Where(t => t.ManualTollClass == Classification).ToList();

            comprehensives = data;

            // === Build dropdown values ===
            TollClasses = [.. data.Select(d => d.ManualTollClass).Where(c => !string.IsNullOrEmpty(c)).Distinct().OrderBy(c => c)];
            Shifts = [.. data.Select(d => d.Shift).Where(s => !string.IsNullOrEmpty(s)).Distinct().OrderBy(s => s)];
            TransactionTypes = [.. data.Select(d => d.TransactionType).Where(t => !string.IsNullOrEmpty(t)).Distinct().OrderBy(t => t)];
            TollOperators = [.. data.Select(d => d.TollOperatorID).Where(o => !string.IsNullOrEmpty(o)).Distinct().OrderBy(o => o)];
            Lanes = [.. data.Select(d => d.LaneName).Where(l => !string.IsNullOrEmpty(l)).Distinct().OrderBy(l => l)];
            PaymentMethods = [.. data.Select(d => d.MethodOfPayment).Where(p => !string.IsNullOrEmpty(p)).Distinct().OrderBy(p => p)];
            DiscountTypes = [.. data.Select(d => d.RowType).Where(d => !string.IsNullOrEmpty(d)).Distinct().OrderBy(d => d)];
            Classifications = TollClasses;

            // === Grouping logic (switchable) ===
            Func<ComprehensiveReportViewModel, string> groupKeySelector =
                GroupBy == "TransactionType"
                    ? t => t.TransactionType ?? "Unknown"
                    : t => t.MethodOfPayment ?? "Unknown";

            GroupedData = data
                .GroupBy(groupKeySelector)
                .Select(g =>
                {
                    var totalCount = g.Count();
                    var totalRevenue = g.Sum(x => x.AmountInclusive);

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
