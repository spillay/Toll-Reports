using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;                  // <<-- ClosedXML for Excel export
using MIS.Web.Models.Comprehensive;
using MIS.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MIS.Web.Pages
{
    public class ComprehensiveReportModel : PageModel
    {
        private readonly IComprehensiveReportService _reportService;

        public ComprehensiveReportModel(IComprehensiveReportService reportService)
        {
            _reportService = reportService;
        }

        // --- Filters (bound to query string via SupportsGet = true) ---
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

        // Grouping option
        [BindProperty(SupportsGet = true)]
        public string GroupBy { get; set; } = "TransactionType";

        // The raw items returned from service (used for Grand Total calculation & reference)
        public List<ComprehensiveReportViewModel> comprehensives { get; set; } = new();

        // Dropdown lists
        public List<string> TollClasses { get; set; } = new();
        public List<string> Shifts { get; set; } = new();
        public List<string> TransactionTypes { get; set; } = new();
        public List<string> TollOperators { get; set; } = new();
        public List<string> Lanes { get; set; } = new();
        public List<string> PaymentMethods { get; set; } = new();
        public List<string> DiscountTypes { get; set; } = new();
        public List<string> Classifications { get; set; } = new();

        // --- Typed grouped data that we will use in both view, PDF (JS) and Excel ---
        public class ClassMetrics
        {
            public int Count { get; set; }
            public decimal CountPercent { get; set; }
            public double Revenue { get; set; }
            public decimal RevenuePercent { get; set; }
        }

        public class GroupedRow
        {
            public string Method { get; set; } = string.Empty;
            public Dictionary<string, ClassMetrics> Classes { get; set; } = new();
            public int TotalCount { get; set; }
            public double TotalRevenue { get; set; }
        }

        public List<GroupedRow> GroupedDataTyped { get; set; } = new();

        // === Page load: builds data and grouping ===
        public async Task OnGetAsync()
        {
            await LoadDataAsync();
        }

        // Encapsulate data retrieval + filtering + grouping so we can re-use for Excel export
        private async Task LoadDataAsync()
        {
            // Get raw data from service (you already had this)
            var data = await _reportService.GetComprehensiveDetailsAsync(StartDate, EndDate);

            // === Apply UI filters ===
            if (!string.IsNullOrEmpty(Shift))
                data = data.Where(t => (t.Shift ?? "").Trim() == Shift).ToList();

            if (!string.IsNullOrEmpty(TransactionType))
                data = data.Where(t => (t.TransactionType ?? "").Trim() == TransactionType).ToList();

            if (!string.IsNullOrEmpty(LaneName))
                data = data.Where(t => (t.LaneName ?? "").Trim() == LaneName).ToList();

            if (!string.IsNullOrEmpty(MethodOfPayment))
                data = data.Where(t => (t.MethodOfPayment ?? "").Trim() == MethodOfPayment).ToList();

            if (!string.IsNullOrEmpty(DiscountType))
                data = data.Where(dt => dt.DiscountType == DiscountType).ToList();

            if (!string.IsNullOrEmpty(Classification))
                data = data.Where(t => t.ManualTollClass == Classification).ToList();

            comprehensives = data.ToList();

            // Build dropdowns
            TollClasses = data.Select(d => d.ManualTollClass).Where(c => !string.IsNullOrEmpty(c)).Distinct().OrderBy(c => c).ToList();
            Shifts = data.Select(d => d.Shift).Where(s => !string.IsNullOrEmpty(s)).Distinct().OrderBy(s => s).ToList();
            TransactionTypes = data.Select(d => d.TransactionType).Where(t => !string.IsNullOrEmpty(t)).Distinct().OrderBy(t => t).ToList();
            TollOperators = data.Select(d => d.TollOperatorID).Where(o => !string.IsNullOrEmpty(o)).Distinct().OrderBy(o => o).ToList();
            Lanes = data.Select(d => d.LaneName).Where(l => !string.IsNullOrEmpty(l)).Distinct().OrderBy(l => l).ToList();
            PaymentMethods = data.Select(d => d.MethodOfPayment).Where(p => !string.IsNullOrEmpty(p)).Distinct().OrderBy(p => p).ToList();
            DiscountTypes = data.Select(d => d.RowType).Where(r => !string.IsNullOrEmpty(r)).Distinct().OrderBy(r => r).ToList();
            Classifications = TollClasses;

            // Grouping key
            Func<ComprehensiveReportViewModel, string> groupKeySelector =
                GroupBy == "TransactionType"
                    ? (Func<ComprehensiveReportViewModel, string>)(t => t.TransactionType ?? "Unknown")
                    : t => t.MethodOfPayment ?? "Unknown";

            // Build typed grouped rows
            GroupedDataTyped = data
                .GroupBy(groupKeySelector)
                .Select(g =>
                {
                    var totalCount = g.Count();
                    var totalRevenue = g.Sum(x => x.AmountInclusive);

                    var classData = TollClasses.ToDictionary(
                        c => c,
                        c =>
                        {
                            var count = g.Count(x => x.ManualTollClass == c);
                            var revenue = g.Where(x => x.ManualTollClass == c).Sum(x => x.AmountInclusive);

                            return new ClassMetrics
                            {
                                Count = count,
                                CountPercent = totalCount == 0 ? 0 : (decimal)count / totalCount * 100,
                                Revenue = revenue,
                                RevenuePercent = totalRevenue == 0 ? 0 : (decimal)revenue / (decimal)totalRevenue * 100
                            };
                        });

                    return new GroupedRow
                    {
                        Method = g.Key,
                        Classes = classData,
                        TotalCount = totalCount,
                        TotalRevenue = totalRevenue
                    };
                })
                .ToList();
        }

        // === Excel export handler (OnGetExport) ===
        public async Task<IActionResult> OnGetExport()
        {
            // Re-load data with the same filter/grouping logic
            await LoadDataAsync();

            // Create Excel workbook with ClosedXML
            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Comprehensive Report");
                int r = 1;

                // Header
                ws.Cell(r, 1).Value = "Comprehensive Report";
                ws.Cell(r, 1).Style.Font.Bold = true;
                ws.Cell(r, 1).Style.Font.FontSize = 16;
                r += 2;

                // Filters summary row
                ws.Cell(r, 1).Value = $"Start Date: {StartDate:dd/MM/yyyy HH:mm}";
                ws.Cell(r, 2).Value = $"End Date: {EndDate:dd/MM/yyyy HH:mm}";
                ws.Cell(r, 4).Value = $"Generated At: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                r += 2;

                // Table header
                int c = 1;
                ws.Cell(r, c++).Value = GroupBy == "TransactionType" ? "TRANSACTION TYPE" : "METHOD OF PAYMENT";
                ws.Cell(r, c++).Value = ""; // labels column
                foreach (var cls in TollClasses)
                {
                    ws.Cell(r, c++).Value = cls;
                }
                ws.Cell(r, c++).Value = "TOTAL";

                // Style header
                var headerRange = ws.Range(r, 1, r, c - 1);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#3498db");
                headerRange.Style.Font.FontColor = XLColor.White;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                r++;

                // Table body: each grouped row (one Excel row with multiline cells)
                foreach (var g in GroupedDataTyped)
                {
                    c = 1;
                    ws.Cell(r, c++).Value = g.Method;
                    var labelCell = ws.Cell(r, c++);
                    labelCell.Value = "Count\nCount %\nRevenue\nRevenue %";
                    labelCell.Style.Alignment.WrapText = true;
                    labelCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                    foreach (var cls in TollClasses)
                    {
                        var cm = g.Classes.ContainsKey(cls) ? g.Classes[cls] : new ClassMetrics();
                        var cell = ws.Cell(r, c++);
                        cell.Value = $"{cm.Count}\n{cm.CountPercent:0.##}\n{cm.Revenue:0.00}\n{cm.RevenuePercent:0.##}";
                        cell.Style.Alignment.WrapText = true;
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }

                    var totalCell = ws.Cell(r, c++);
                    totalCell.Value = $"{g.TotalCount}\n100\n{g.TotalRevenue:0.00}\n100";
                    totalCell.Style.Alignment.WrapText = true;
                    totalCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    r++;
                }

                // Grand Total row
                ws.Cell(r, 1).Value = "Grand Total";
                ws.Cell(r, 2).Value = "Count\nCount %\nRevenue\nRevenue %";
                ws.Cell(r, 2).Style.Alignment.WrapText = true;
                int cc = 3;
                foreach (var cls in TollClasses)
                {
                    var tc = comprehensives.Count(x => x.ManualTollClass == cls);
                    var tr = comprehensives.Where(x => x.ManualTollClass == cls).Sum(x => x.AmountInclusive);
                    ws.Cell(r, cc++).Value = $"{tc}\n100\n{tr:0.00}\n100";
                    ws.Cell(r, cc - 1).Style.Alignment.WrapText = true;
                }
                var grandTotalCount = comprehensives.Count();
                var grandTotalRevenue = comprehensives.Sum(x => x.AmountInclusive);
                ws.Cell(r, cc++).Value = $"{grandTotalCount}\n100\n{grandTotalRevenue:0.00}\n100";

                // Auto-fit columns (wrap text will still be set)
                ws.Columns().AdjustToContents();

                // Export to memory stream and return as file
                using (var ms = new MemoryStream())
                {
                    wb.SaveAs(ms);
                    ms.Position = 0;
                    var fileName = $"ComprehensiveReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }
    }
}
