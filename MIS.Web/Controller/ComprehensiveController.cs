using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models.Comprehensive;
using MIS.Web.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace MIS.Web.Controllers
{
    public class ComprehensiveController : Controller
    {
        private readonly IComprehensiveReportService _reportService;

        public ComprehensiveController(IComprehensiveReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<IActionResult> Index([FromQuery] ComprehensiveInputModel input)
        {
            // ✅ Ensure all filters default to empty (meaning "All")
            input ??= new ComprehensiveInputModel();
            input.Shift ??= "";
            input.TransactionType ??= "";
            input.LaneName ??= "";
            input.MethodOfPayment ??= "";
            input.DiscountType ??= "";
            input.Classification ??= "";
            input.TollOperatorID ??= ""; // ✅ renamed from TollOperatorID

            var pageModel = await BuildPageModelAsync(input);
            return View("~/Views/Comprehensive/Index.cshtml", pageModel);
        }

        public async Task<IActionResult> ExportExcel([FromQuery] ComprehensiveInputModel input)
        {
            var pageModel = await BuildPageModelAsync(input);

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Comprehensive Report");
            int r = 1;

            ws.Cell(r, 1).Value = "Comprehensive Report";
            ws.Cell(r, 1).Style.Font.Bold = true;
            ws.Cell(r, 1).Style.Font.FontSize = 16;
            r += 2;

            ws.Cell(r, 1).Value = $"Start Date: {input.StartDate:dd/MM/yyyy HH:mm}";
            ws.Cell(r, 2).Value = $"End Date: {input.EndDate:dd/MM/yyyy HH:mm}";
            ws.Cell(r, 4).Value = $"Generated At: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
            r += 2;

            int c = 1;
            ws.Cell(r, c++).Value = input.GroupBy == "TransactionType" ? "TRANSACTION TYPE" : "METHOD OF PAYMENT";
            ws.Cell(r, c++).Value = "";
            foreach (var cls in pageModel.TollClasses)
                ws.Cell(r, c++).Value = cls;
            ws.Cell(r, c++).Value = "TOTAL";

            var headerRange = ws.Range(r, 1, r, c - 1);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromHtml("#3498db");
            headerRange.Style.Font.FontColor = ClosedXML.Excel.XLColor.White;
            headerRange.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
            r++;

            foreach (var g in pageModel.GroupedDataTyped)
            {
                c = 1;
                ws.Cell(r, c++).Value = g.Method;
                var labelCell = ws.Cell(r, c++);
                labelCell.Value = "Count\nCount %\nRevenue\nRevenue %";
                labelCell.Style.Alignment.WrapText = true;
                labelCell.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

                foreach (var cls in pageModel.TollClasses)
                {
                    var cm = g.Classes.ContainsKey(cls) ? g.Classes[cls] : new PageComprehensiveModel.ClassMetrics();
                    var cell = ws.Cell(r, c++);
                    cell.Value = $"{cm.Count}\n{cm.CountPercent:0.##}\n{cm.Revenue:0.00}\n{cm.RevenuePercent:0.##}";
                    cell.Style.Alignment.WrapText = true;
                    cell.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                }

                var totalCell = ws.Cell(r, c++);
                totalCell.Value = $"{g.TotalCount}\n100\n{g.TotalRevenue:0.00}\n100";
                totalCell.Style.Alignment.WrapText = true;
                totalCell.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                r++;
            }

            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            ms.Position = 0;

            return File(ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"ComprehensiveReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }

        // ✅ Build Model + Filters + Dropdowns
        private async Task<PageComprehensiveModel> BuildPageModelAsync(ComprehensiveInputModel input)
        {
            var data = await _reportService.GetComprehensiveDetailsAsync(input.StartDate, input.EndDate);

            // --- Apply filters ---
            if (!string.IsNullOrWhiteSpace(input.Shift))
                data = data.Where(t => string.Equals(t.Shift, input.Shift, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(input.TransactionType))
                data = data.Where(t => string.Equals(t.TransactionType, input.TransactionType, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(input.LaneName))
                data = data.Where(t => string.Equals(t.LaneName, input.LaneName, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(input.MethodOfPayment))
                data = data.Where(t => string.Equals(t.MethodOfPayment, input.MethodOfPayment, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(input.DiscountType))
                data = data.Where(dt => string.Equals(dt.DiscountType, input.DiscountType, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(input.TollOperatorID))
                data = data.Where(o => string.Equals(o.TollOperatorID, input.TollOperatorID, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrEmpty(input.Classification))
            {
                var normalized = input.Classification.Trim().ToLowerInvariant();
                data = data.Where(t =>
                    (t.ManualTollClass ?? "").Trim().ToLowerInvariant() == normalized ||
                    ((t.ManualTollClass ?? "").Trim().ToLowerInvariant() == "motor cycle" && normalized == "class m")
                ).ToList();
            }

            // --- Prepare Page Model ---
            var pageModel = new PageComprehensiveModel
            {
                Input = input,
                Items = data
            };

            // --- Dropdown Lists ---
            pageModel.TollClasses = data.Select(d => d.ManualTollClass).Where(c => !string.IsNullOrEmpty(c)).Distinct().OrderBy(c => c).ToList();
            pageModel.Shifts = data.Select(d => d.Shift).Where(s => !string.IsNullOrEmpty(s)).Distinct().OrderBy(s => s).ToList();
            pageModel.TransactionTypes = data.Select(d => d.TransactionType).Where(t => !string.IsNullOrEmpty(t)).Distinct().OrderBy(t => t).ToList();
            pageModel.TollOperators = data.Select(d => d.TollOperatorID).Where(o => !string.IsNullOrEmpty(o)).Distinct().OrderBy(o => o).ToList(); // ✅ fixed
            pageModel.Lanes = data.Select(d => d.LaneName).Where(l => !string.IsNullOrEmpty(l)).Distinct().OrderBy(l => l).ToList();
            pageModel.PaymentMethods = data.Select(d => d.MethodOfPayment).Where(p => !string.IsNullOrEmpty(p)).Distinct().OrderBy(p => p).ToList();
            pageModel.DiscountTypes = data.Select(d => d.DiscountType).Where(r => !string.IsNullOrEmpty(r)).Distinct().OrderBy(r => r).ToList();
            pageModel.Classifications = pageModel.TollClasses;

            // --- Grouping ---
            Func<ComprehensiveModel, string> groupKeySelector =
                input.GroupBy == "TransactionType"
                    ? (Func<ComprehensiveModel, string>)(t => t.TransactionType ?? "Unknown")
                    : t => t.MethodOfPayment ?? "Unknown";

            pageModel.GroupedDataTyped = data
                .GroupBy(groupKeySelector)
                .Select(g =>
                {
                    var totalCount = g.Count();
                    var totalRevenue = g.Sum(x => x.AmountInclusive);

                    var classData = pageModel.TollClasses.ToDictionary(
                        c => c,
                        c =>
                        {
                            var count = g.Count(x => x.ManualTollClass == c);
                            var revenue = g.Where(x => x.ManualTollClass == c).Sum(x => x.AmountInclusive);

                            return new PageComprehensiveModel.ClassMetrics
                            {
                                Count = count,
                                CountPercent = totalCount == 0 ? 0 : (decimal)count / totalCount * 100,
                                Revenue = revenue,
                                RevenuePercent = totalRevenue == 0 ? 0 : (decimal)revenue / (decimal)totalRevenue * 100
                            };
                        });

                    return new PageComprehensiveModel.GroupedRow
                    {
                        Method = g.Key,
                        Classes = classData,
                        TotalCount = totalCount,
                        TotalRevenue = totalRevenue
                    };
                })
                .OrderBy(r => r.Method, StringComparer.OrdinalIgnoreCase)
                .ToList();

            return pageModel;
        }
    }
}
