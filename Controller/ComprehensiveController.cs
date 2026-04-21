using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models.Comprehensive;
using MIS.Web.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MIS.Web.Controllers
{
    [Authorize]
    public class ComprehensiveController : Controller
    {
        private readonly IComprehensiveReportService _reportService;

        public ComprehensiveController(IComprehensiveReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<IActionResult> Index([FromQuery] ComprehensiveInputModel input)
        {
            input ??= new ComprehensiveInputModel();

            // Defensive (never null)
            input.ShiftIds ??= new List<byte>();
            input.OperatorIds ??= new List<long>();
            input.LaneIds ??= new List<int>();
            input.DiscountTypeIds ??= new List<byte>();
            input.TollClassIds ??= new List<byte>();
            input.PaymentMethodIds ??= new List<byte>();

            var pageModel = await BuildPageModelAsync(input);
            return View("~/Views/Comprehensive/Index.cshtml", pageModel);
        }

        public async Task<IActionResult> ExportExcel([FromQuery] ComprehensiveInputModel input)
        {
            input ??= new ComprehensiveInputModel();

            input.ShiftIds ??= new List<byte>();
            input.OperatorIds ??= new List<long>();
            input.LaneIds ??= new List<int>();
            input.DiscountTypeIds ??= new List<byte>();
            input.TollClassIds ??= new List<byte>();
            input.PaymentMethodIds ??= new List<byte>();

            var pageModel = await BuildPageModelAsync(input);

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Comprehensive Report");

            int r = 1;

            ws.Cell(r, 1).Value = "Comprehensive Report";
            ws.Cell(r, 1).Style.Font.Bold = true;
            ws.Cell(r, 1).Style.Font.FontSize = 16;
            r += 2;

            ws.Cell(r, 1).Value = $"Start Date: {input.StartDate:dd/MM/yyyy}";
            ws.Cell(r, 2).Value = $"End Date: {input.EndDate:dd/MM/yyyy}";
            ws.Cell(r, 4).Value = $"Generated At: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
            r++;

            ws.Cell(r, 1).Value = $"Shift(s): {pageModel.FilterTextOperationalShift}";
            ws.Cell(r, 2).Value = $"Operator(s): {pageModel.FilterTextOperators}";
            ws.Cell(r, 3).Value = $"Lane(s): {pageModel.FilterTextLanes}";
            ws.Cell(r, 4).Value = $"Payment Method(s): {pageModel.FilterTextPaymentMethods}";
            r += 2;

            var groupByLabel = GetGroupByLabel(input.GroupBy);

            int c = 1;
            ws.Cell(r, c++).Value = groupByLabel;
            ws.Cell(r, c++).Value = "";
            foreach (var cls in pageModel.TollClasses)
                ws.Cell(r, c++).Value = (cls == "Motor Cycle" ? "Class M" : cls);
            ws.Cell(r, c++).Value = "TOTAL";

            var headerRange = ws.Range(r, 1, r, c - 1);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(41, 128, 185); // consistent
            headerRange.Style.Font.FontColor = XLColor.White;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            r++;

            foreach (var g in pageModel.GroupedDataTyped)
            {
                c = 1;
                ws.Cell(r, c++).Value = g.Method;

                var labelCell = ws.Cell(r, c++);
                labelCell.Value = "Count\nCount %\nRevenue\nRevenue %";
                labelCell.Style.Alignment.WrapText = true;
                labelCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                foreach (var cls in pageModel.TollClasses)
                {
                    var cm = g.Classes.TryGetValue(cls, out var m)
                        ? m
                        : new PageComprehensiveModel.ClassMetrics();

                    var cell = ws.Cell(r, c++);
                    cell.Value = $"{cm.Count}\n{cm.CountPercent:0.##}%\n{cm.Revenue:0.00}\n{cm.RevenuePercent:0.##}%";
                    cell.Style.Alignment.WrapText = true;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                // ✅ NO hardcoded 100 here
                var totalCell = ws.Cell(r, c++);
                totalCell.Value = $"{g.TotalCount}\n{g.TotalCountPercent:0.##}%\n{g.TotalRevenue:0.00}\n{g.TotalRevenuePercent:0.##}%";
                totalCell.Style.Alignment.WrapText = true;
                totalCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

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

        private async Task<PageComprehensiveModel> BuildPageModelAsync(ComprehensiveInputModel input)
        {
            // ✅ Load ALL filter values (not date filtered)
            var options = await _reportService.GetComprehensiveOptionsAsync();

            // ✅ Fetch report data using IDs (API filters)
            var data = await _reportService.GetComprehensiveDetailsAsync(
                input.StartDate,
                input.EndDate,
                input.ShiftIds,
                input.OperatorIds,
                input.LaneIds,
                input.DiscountTypeIds,
                input.TollClassIds,
                input.PaymentMethodIds
            );

            var page = new PageComprehensiveModel
            {
                Input = input,
                Items = data,

                Shifts = options.Shifts,
                TollOperators = options.Operators,
                Lanes = options.Lanes,
                DiscountTypes = options.DiscountTypes,
                Classifications = options.TollClasses,
                PaymentMethods = options.PaymentMethods
            };

            // Matrix columns (all classes, from master options)
            page.TollClasses = page.Classifications
                .Select(x => x.Name)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Export filter text (names from selected IDs)
            page.FilterTextOperationalShift = BuildSelectedText(page.Shifts, input.ShiftIds);
            page.FilterTextOperators = BuildSelectedText(page.TollOperators, input.OperatorIds);
            page.FilterTextLanes = BuildSelectedText(page.Lanes, input.LaneIds);
            page.FilterTextPaymentMethods = BuildSelectedText(page.PaymentMethods, input.PaymentMethodIds);
            page.FilterTextDiscountTypes = BuildSelectedText(page.DiscountTypes, input.DiscountTypeIds);
            page.FilterTextClassifications = BuildSelectedText(page.Classifications, input.TollClassIds);

            // Grouping selector
            Func<ComprehensiveModel, string> groupKey = input.GroupBy switch
            {
                "Shift" => x => x.ShiftName ?? "Unknown",
                "Lane" => x => x.LaneName ?? "Unknown",
                "DiscountType" => x => x.DiscountTypeName ?? "Unknown",
                "Classification" => x => x.ManualTollClassName ?? "Unknown",
                "TransactionType" => x => x.PaymentMethodName ?? "Unknown", // your mapping
                "MethodOfPayment" => x => x.PaymentMethodName ?? "Unknown",
                _ => x => x.PaymentMethodName ?? "Unknown"
            };

            // Grand totals (safe)
            var grandCount = data.Count;
            var grandRevenue = data.Sum(x => x.AmountInclusive ?? 0);

            var safeGrandCount = grandCount == 0 ? 1 : grandCount;
            var safeGrandRevenue = Math.Abs(grandRevenue) < 0.0000001 ? 1 : grandRevenue;

            page.GroupedDataTyped = data
                .GroupBy(groupKey)
                .Select(g =>
                {
                    var rowCount = g.Count();
                    var rowRevenue = g.Sum(x => x.AmountInclusive ?? 0);

                    // ✅ Row totals as % of GRAND totals
                    var rowCountPct = (decimal)rowCount / safeGrandCount * 100m;
                    var rowRevenuePct = (decimal)rowRevenue / (decimal)safeGrandRevenue * 100m;

                    // ✅ Class cells as % of GRAND totals (NOT row totals)
                    var classData = page.TollClasses.ToDictionary(
                        cls => cls,
                        cls =>
                        {
                            var count = g.Count(x => string.Equals(x.ManualTollClassName, cls, StringComparison.OrdinalIgnoreCase));
                            var revenue = g.Where(x => string.Equals(x.ManualTollClassName, cls, StringComparison.OrdinalIgnoreCase))
                                           .Sum(x => x.AmountInclusive ?? 0);

                            var countPct = (decimal)count / safeGrandCount * 100m;
                            var revenuePct = (decimal)revenue / (decimal)safeGrandRevenue * 100m;

                            return new PageComprehensiveModel.ClassMetrics
                            {
                                Count = count,
                                CountPercent = countPct,
                                Revenue = revenue,
                                RevenuePercent = revenuePct
                            };
                        });

                    return new PageComprehensiveModel.GroupedRow
                    {
                        Method = g.Key,
                        Classes = classData,
                        TotalCount = rowCount,
                        TotalRevenue = rowRevenue,
                        TotalCountPercent = rowCountPct,
                        TotalRevenuePercent = rowRevenuePct
                    };
                })
                .OrderBy(x => x.Method, StringComparer.OrdinalIgnoreCase)
                .ToList();

            return page;
        }

        private static string GetGroupByLabel(string? groupBy) =>
            groupBy switch
            {
                "Shift" => "OPERATIONAL SHIFT",
                "Lane" => "LANE",
                "DiscountType" => "DISCOUNT TYPE",
                "Classification" => "CLASSIFICATION",
                "TransactionType" => "METHOD OF PAYMENT",
                "MethodOfPayment" => "METHOD OF PAYMENT",
                _ => "METHOD OF PAYMENT"
            };

        private static string BuildSelectedText<TId>(IEnumerable<PageComprehensiveModel.FilterOption<TId>> options, IEnumerable<TId> selectedIds)
        {
            if (selectedIds == null || !selectedIds.Any())
                return "All";

            var set = new HashSet<TId>(selectedIds);
            var names = options.Where(o => set.Contains(o.Id))
                               .Select(o => o.Name)
                               .Where(n => !string.IsNullOrWhiteSpace(n))
                               .Distinct(StringComparer.OrdinalIgnoreCase)
                               .ToList();

            return names.Count == 0 ? "All" : string.Join(", ", names);
        }
    }
}