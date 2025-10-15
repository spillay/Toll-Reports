//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using MIS.Web.Models.Transaction;
//using MIS.Web.Services;
//using OfficeOpenXml;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;

//namespace MIS.Web.Pages.Reports
//{
//    public class TransactionReportModel : PageModel
//    {
//        private readonly IReportService _reportService;

//        public TransactionReportModel(IReportService reportService)
//        {
//            _reportService = reportService;
//        }

//        // Bound filters
//        [BindProperty(SupportsGet = true)] public DateTime StartDate { get; set; } = DateTime.Now.AddDays(-90);
//        [BindProperty(SupportsGet = true)] public DateTime EndDate { get; set; } = DateTime.Now;
//        [BindProperty(SupportsGet = true)] public string? lane_Nr { get; set; }
//        [BindProperty(SupportsGet = true)] public string? TollOperatorID { get; set; }
//        [BindProperty(SupportsGet = true)] public string? Shift { get; set; }
//        [BindProperty(SupportsGet = true)] public string? PaymentMethod { get; set; }
//        [BindProperty(SupportsGet = true)] public string? SortOrder { get; set; }

//        public List<TransactionModel> Transactions { get; set; } = new();
//        public int TotalRowCount { get; set; }
//        public decimal TotalTariffSum { get; set; }
//        public int FilteredRowCount { get; set; }
//        public decimal FilteredTariffSum { get; set; }

//        //public async Task OnGetAsync()
//        //{
//        //    var allTransactions = await _reportService.GetTransactionDetailsAsync(1, 10, StartDate, EndDate);
//        //    allTransactions = ApplyFilters(allTransactions);

//        //    TotalRowCount = allTransactions.Count;
//        //    TotalTariffSum = allTransactions.Sum(t => t.tariff);

//        //    var filtered = allTransactions.Where(t => t.TransactionDateTime >= StartDate && t.TransactionDateTime <= EndDate).ToList();

//        //    FilteredRowCount = filtered.Count;
//        //    FilteredTariffSum = filtered.Sum(t => t.tariff);

//        //    Transactions = SortTransactions(filtered);
//        //}

//        private List<TransactionModel> ApplyFilters(IEnumerable<TransactionModel> data)
//        {
//            return data
//                .Where(t => string.IsNullOrEmpty(Shift) || t.operational_Shift == Shift)
//                .Where(t => string.IsNullOrEmpty(PaymentMethod) || t.method_of_Payment?.Trim() == PaymentMethod.Trim())
//                .Where(t => string.IsNullOrEmpty(TollOperatorID) || t.toll_Operator_ID == TollOperatorID)
//                .Where(t => string.IsNullOrEmpty(lane_Nr) || t.lane_Nr == lane_Nr)
//                .ToList();
//        }

//        private List<TransactionModel> SortTransactions(IEnumerable<TransactionModel> data) =>
//            SortOrder switch
//            {
//                "lane_Nr" => data.OrderBy(t => t.lane_Nr).ToList(),
//                "lane_Nr_desc" => data.OrderByDescending(t => t.lane_Nr).ToList(),
//                "trx_Sequence_Nr" => data.OrderBy(t => t.trx_Sequence_Nr).ToList(),
//                "trx_Sequence_Nr_desc" => data.OrderByDescending(t => t.trx_Sequence_Nr).ToList(),
//                "TransactionDateTime" => data.OrderBy(t => t.TransactionDateTime).ToList(),
//                "TransactionDateTime_desc" => data.OrderByDescending(t => t.TransactionDateTime).ToList(),
//                "tariff" => data.OrderBy(t => t.tariff).ToList(),
//                "tariff_desc" => data.OrderByDescending(t => t.tariff).ToList(),
//                _ => data.OrderByDescending(t => t.TransactionDateTime).ToList()
//            };

//        public async Task<IActionResult> OnGetExportExcelAsync()
//        {
//            //var data = ApplyFilters(await _reportService.GetTransactionDetailsAsync(1, 10, StartDate, EndDate));

//            using var package = new ExcelPackage();
//            var ws = package.Workbook.Worksheets.Add("Transactions");

//            var headers = new[] { "Lane", "Transaction #", "Date", "Time", "Shift", "Operator", "Lane Name",
//                "Payment", "Collector Class", "AVC Class", "Final Class", "Amount", "Card Number" };

//            for (int i = 0; i < headers.Length; i++)
//            {
//                ws.Cells[1, i + 1].Value = headers[i];
//                ws.Cells[1, i + 1].Style.Font.Bold = true;
//            }

//            //for (int i = 0; i < data.Count; i++)
//            //{
//            //    var t = data[i];
//            //    ws.Cells[i + 2, 1].Value = t.lane_Nr;
//            //    ws.Cells[i + 2, 2].Value = t.trx_Sequence_Nr;
//            //    ws.Cells[i + 2, 3].Value = t.TransactionDateTime.ToString("dd/MM/yyyy");
//            //    ws.Cells[i + 2, 4].Value = t.TransactionDateTime.ToString("HH:mm:ss");
//            //    ws.Cells[i + 2, 5].Value = t.operational_Shift;
//            //    ws.Cells[i + 2, 6].Value = t.toll_Operator_ID;
//            //    ws.Cells[i + 2, 7].Value = t.lane_Name;
//            //    ws.Cells[i + 2, 8].Value = t.method_of_Payment;
//            //    ws.Cells[i + 2, 9].Value = t.toll_Collector_Class;
//            //    ws.Cells[i + 2, 10].Value = t.avC_Class;
//            //    ws.Cells[i + 2, 11].Value = t.final_Class;
//            //    ws.Cells[i + 2, 12].Value = t.tariff;
//            //    ws.Cells[i + 2, 13].Value = t.tac_Card_Number;
//            //}

//            ws.Cells[ws.Dimension.Address].AutoFitColumns();

//            var stream = new MemoryStream();
//            package.SaveAs(stream);
//            stream.Position = 0;

//            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TransactionReport.xlsx");
//        }
//    }
//}
