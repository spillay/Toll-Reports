using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MIS.Web.Models.Transaction;
using MIS.Web.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using OfficeOpenXml;

namespace MIS.Web.Pages.Reports
{
    public class TransactionReportModel : PageModel
    {
        private readonly IReportService _reportService;

        public TransactionReportModel(IReportService reportService)
        {
            _reportService = reportService;
        }

        // Filters
        [BindProperty(SupportsGet = true)]
        public DateTime StartDate { get; set; } = DateTime.Now.AddDays(-90);

        [BindProperty(SupportsGet = true)]
        public DateTime EndDate { get; set; } = DateTime.Now;

        [BindProperty(SupportsGet = true)]
        public string? lane_Nr { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? TollOperatorID { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Shift { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? PaymentMethod { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SortOrder { get; set; }

        // Pagination
        public int PageSize { get; set; } = 10;

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int TotalPages { get; set; }

        public List<TransactionReportViewModel> Transactions { get; set; } = new();

        // Totals
        public int TotalRowCount { get; set; }
        public decimal TotalTariffSum { get; set; }
        public int FilteredRowCount { get; set; }
        public decimal FilteredTariffSum { get; set; }

        public string GetSortOrder(string column)
        {
            if (string.IsNullOrEmpty(SortOrder)) return column;
            if (SortOrder == column) return column + "_desc";
            if (SortOrder == column + "_desc") return column;
            return column;
        }

        public async Task OnGetAsync()
        {
            // Fetch all transactions from the service
            var allTransactions = await _reportService.GetTransactionDetailsAsync(StartDate, EndDate);

            // Apply filters manually
            if (!string.IsNullOrEmpty(Shift))
                allTransactions = allTransactions.Where(t => t.operational_Shift == Shift).ToList();

            if (!string.IsNullOrEmpty(PaymentMethod))
                allTransactions = allTransactions.Where(t => t.method_of_Payment == PaymentMethod).ToList();

            if (!string.IsNullOrEmpty(TollOperatorID))
                allTransactions = allTransactions.Where(t => t.toll_Operator_ID == TollOperatorID).ToList();

            if (!string.IsNullOrEmpty(lane_Nr))
                allTransactions = allTransactions.Where(t => t.lane_Nr == lane_Nr).ToList();


            // Totals
            TotalRowCount = allTransactions.Count;
            TotalTariffSum = allTransactions.Sum(t => t.tariff);

            // Apply filters for current page
            var query = allTransactions
                .Where(t => t.TransactionDateTime >= StartDate && t.TransactionDateTime <= EndDate)
                .AsQueryable();

            FilteredRowCount = query.Count();
            FilteredTariffSum = query.Sum(t => t.tariff);

            // Sorting
            query = SortOrder switch
            {
                "lane_Nr" => query.OrderBy(t => t.lane_Nr),
                "lane_Nr_desc" => query.OrderByDescending(t => t.lane_Nr),
                "trx_Sequence_Nr" => query.OrderBy(t => t.trx_Sequence_Nr),
                "trx_Sequence_Nr_desc" => query.OrderByDescending(t => t.trx_Sequence_Nr),
                "TransactionDateTime" => query.OrderBy(t => t.TransactionDateTime),
                "TransactionDateTime_desc" => query.OrderByDescending(t => t.TransactionDateTime),
                "tariff" => query.OrderBy(t => t.tariff),
                "tariff_desc" => query.OrderByDescending(t => t.tariff),
                _ => query.OrderByDescending(t => t.TransactionDateTime)
            };

            // Pagination
            var totalRecords = query.Count();
            TotalPages = (int)Math.Ceiling(totalRecords / (double)PageSize);

            Transactions = query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }

        // Excel export
        public async Task<IActionResult> OnGetExportExcelAsync()
        {
            // Fetch all transactions within the date range
            var transactions = await _reportService.GetTransactionDetailsAsync(StartDate, EndDate);

            // Apply filters in-memory
            if (!string.IsNullOrEmpty(Shift))
                transactions = transactions.Where(t => t.operational_Shift == Shift).ToList();

            if (!string.IsNullOrEmpty(PaymentMethod))
                transactions = transactions.Where(t => t.method_of_Payment == PaymentMethod).ToList();

            if (!string.IsNullOrEmpty(TollOperatorID))
                transactions = transactions.Where(t => t.toll_Operator_ID == TollOperatorID).ToList();

            if (!string.IsNullOrEmpty(lane_Nr))
                transactions = transactions.Where(t => t.lane_Nr == lane_Nr).ToList();


            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Transactions");

            // Headers
            string[] headers = { "Lane", "Transaction #", "Date", "Time", "Shift", "Operator", "Lane Name",
                "Payment", "Collector Class", "AVC Class", "Final Class", "Amount", "Card Number" };
            for (int col = 0; col < headers.Length; col++)
            {
                ws.Cells[1, col + 1].Value = headers[col];
                ws.Cells[1, col + 1].Style.Font.Bold = true;
            }

            // Rows
            for (int i = 0; i < transactions.Count; i++)
            {
                var t = transactions[i];
                ws.Cells[i + 2, 1].Value = t.lane_Nr;
                ws.Cells[i + 2, 2].Value = t.trx_Sequence_Nr;
                ws.Cells[i + 2, 3].Value = t.TransactionDateTime.ToString("dd/MM/yyyy");
                ws.Cells[i + 2, 4].Value = t.TransactionDateTime.ToString("HH:mm:ss");
                ws.Cells[i + 2, 5].Value = t.operational_Shift;
                ws.Cells[i + 2, 6].Value = t.toll_Operator_ID;
                ws.Cells[i + 2, 7].Value = t.lane_Name;
                ws.Cells[i + 2, 8].Value = t.method_of_Payment;
                ws.Cells[i + 2, 9].Value = t.toll_Collector_Class;
                ws.Cells[i + 2, 10].Value = t.avC_Class;
                ws.Cells[i + 2, 11].Value = t.final_Class;
                ws.Cells[i + 2, 12].Value = t.tariff;
                ws.Cells[i + 2, 13].Value = t.tac_Card_Number;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            return File(stream,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "TransactionReport.xlsx");
        }
    }
}
