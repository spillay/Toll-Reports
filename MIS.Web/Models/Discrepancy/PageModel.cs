using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MIS.Models;
using MIS.Web.Models.Discrepancy;
using MIS.Web.Services;
using System.IO;

public class DiscrepancyReportModel : PageModel
{
    private readonly IDiscrepancyReportService _reportService;

    public DiscrepancyReportModel(IDiscrepancyReportService reportService)
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
    public string? Shift { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? PaymentMethod { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? takenAction { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SortOrder { get; set; }

    // Pagination
    public int PageSize { get; set; } = 10;
    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;
    public int TotalPages { get; set; }

    public List<DiscrepancyReportViewModel> Discrepancys { get; set; } = new();
    public List<DiscrepancyReportViewModel> AllDiscrepancys { get; set; } = new();

    public string GetSortOrder(string column)
    {
        if (string.IsNullOrEmpty(SortOrder)) return column;         // Default ascending
        if (SortOrder == column) return column + "_desc";           // Toggle to descending
        if (SortOrder == column + "_desc") return column;           // Toggle back to ascending
        return column;
    }

    public async Task OnGetAsync()
    {
        var query = (await _reportService.GetDiscrepancyDetailsAsync(StartDate, EndDate)).AsQueryable();

        // Filter by combined Date & Time
        query = query.Where(t => t.TransactionDateTime >= StartDate && t.TransactionDateTime <= EndDate);

        // Other filters
        if (!string.IsNullOrEmpty(lane_Nr)) query = query.Where(t => t.lane_Nr == lane_Nr);
        if (!string.IsNullOrEmpty(Shift)) query = query.Where(t => t.operational_Shift == Shift);
        if (!string.IsNullOrEmpty(PaymentMethod)) query = query.Where(t => t.method_of_Payment == PaymentMethod);
        if (!string.IsNullOrEmpty(takenAction))
        {
            query = takenAction switch
            {
                "Both Correct" => query.Where(t => t.toll_Collector_Class == t.final_Class && t.avC_Class == t.final_Class),
                "Operator Correct" => query.Where(t => t.toll_Collector_Class == t.final_Class && t.avC_Class != t.final_Class),
                "AVC Correct" => query.Where(t => t.avC_Class == t.final_Class && t.toll_Collector_Class != t.final_Class),
                "Both Incorrect" => query.Where(t => t.toll_Collector_Class != t.final_Class && t.avC_Class != t.final_Class),
                _ => query
            };
        }

        {
            if (!string.IsNullOrEmpty(takenAction))
            {
                switch (takenAction)
                {
                    case "Both Correct":
                        query = query.Where(t => t.toll_Collector_Class == t.final_Class
                                             && t.avC_Class == t.final_Class);
                        break;
                    case "Operator Correct":
                        query = query.Where(t => t.toll_Collector_Class == t.final_Class
                                             && t.avC_Class != t.final_Class);
                        break;
                    case "AVC Correct":
                        query = query.Where(t => t.avC_Class == t.final_Class
                                             && t.toll_Collector_Class != t.final_Class);
                        break;
                    case "Both Incorrect":
                        query = query.Where(t => t.toll_Collector_Class != t.final_Class
                                             && t.avC_Class != t.final_Class);
                        break;
                }
            }

        }

        // Sorting
        query = SortOrder switch
        {
            "lane_Nr" => query.OrderBy(t => t.lane_Nr),
            "lane_Nr_desc" => query.OrderByDescending(t => t.lane_Nr),
            "trx_Sequence_Nr" => query.OrderBy(t => t.trx_Sequence_Nr),
            "trx_Sequence_Nr_desc" => query.OrderByDescending(t => t.trx_Sequence_Nr),
            "TransactionDateTime" => query.OrderBy(t => t.TransactionDateTime),
            "TransactionDateTime_desc" => query.OrderByDescending(t => t.TransactionDateTime),
            "operational_Shift" => query.OrderBy(t => t.operational_Shift),
            "operational_Shift_desc" => query.OrderByDescending(t => t.operational_Shift),
            "toll_Operator_ID" => query.OrderBy(t => t.toll_Operator_ID),
            "toll_Operator_ID_desc" => query.OrderByDescending(t => t.toll_Operator_ID),
            _ => query.OrderByDescending(t => t.TransactionDateTime)
        };

        AllDiscrepancys = query.ToList(); // Full list for summary
        var totalRecords = AllDiscrepancys.Count;
        TotalPages = (int)Math.Ceiling(totalRecords / (double)PageSize);

        Discrepancys = AllDiscrepancys
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToList();
    }

    
public async Task<IActionResult> OnGetExportAsync()
{
    var query = (await _reportService.GetDiscrepancyDetailsAsync(StartDate, EndDate)).AsQueryable();

    // Apply same filters as in OnGetAsync
    query = query.Where(t => t.TransactionDateTime >= StartDate && t.TransactionDateTime <= EndDate);
    if (!string.IsNullOrEmpty(lane_Nr)) query = query.Where(t => t.lane_Nr == lane_Nr);
    if (!string.IsNullOrEmpty(Shift)) query = query.Where(t => t.operational_Shift == Shift);
    if (!string.IsNullOrEmpty(PaymentMethod)) query = query.Where(t => t.method_of_Payment == PaymentMethod);
    if (!string.IsNullOrEmpty(takenAction))
    {
        query = takenAction switch
        {
            "Both Correct" => query.Where(t => t.toll_Collector_Class == t.final_Class && t.avC_Class == t.final_Class),
            "Operator Correct" => query.Where(t => t.toll_Collector_Class == t.final_Class && t.avC_Class != t.final_Class),
            "AVC Correct" => query.Where(t => t.avC_Class == t.final_Class && t.toll_Collector_Class != t.final_Class),
            "Both Incorrect" => query.Where(t => t.toll_Collector_Class != t.final_Class && t.avC_Class != t.final_Class),
            _ => query
        };
    }

    var data = query.ToList();

    using var workbook = new XLWorkbook();
    var worksheet = workbook.Worksheets.Add("Discrepancy Report");

    // Add headers
    worksheet.Cell(1, 1).Value = "Lane";
    worksheet.Cell(1, 2).Value = "Trx #";
    worksheet.Cell(1, 3).Value = "Date";
    worksheet.Cell(1, 4).Value = "Time";
    worksheet.Cell(1, 5).Value = "Shift";
    worksheet.Cell(1, 6).Value = "Operator";
    worksheet.Cell(1, 7).Value = "Payment";
    worksheet.Cell(1, 8).Value = "Collector Class";
    worksheet.Cell(1, 9).Value = "AVC Class";
    worksheet.Cell(1, 10).Value = "Final Class";
    worksheet.Cell(1, 11).Value = "Tariff";
    worksheet.Cell(1, 12).Value = "Updated Tariff";
    worksheet.Cell(1, 13).Value = "Difference";
    worksheet.Cell(1, 14).Value = "Action Taken";

    // Add data
    int row = 2;
    foreach (var t in data)
    {
        worksheet.Cell(row, 1).Value = t.lane_Nr;
        worksheet.Cell(row, 2).Value = t.trx_Sequence_Nr;
        worksheet.Cell(row, 3).Value = t.trx_Date;
        worksheet.Cell(row, 4).Value = t.trx_Time;
        worksheet.Cell(row, 5).Value = t.operational_Shift;
        worksheet.Cell(row, 6).Value = t.toll_Operator_ID;
        worksheet.Cell(row, 7).Value = t.method_of_Payment;
        worksheet.Cell(row, 8).Value = t.toll_Collector_Class;
        worksheet.Cell(row, 9).Value = t.avC_Class;
        worksheet.Cell(row, 10).Value = t.final_Class;
        worksheet.Cell(row, 11).Value = t.tariff;
        worksheet.Cell(row, 12).Value = t.updated_tariff;
        worksheet.Cell(row, 13).Value = t.updated_tariff - t.tariff;

        string action = "No Action Taken";
        bool tollExists = !string.IsNullOrEmpty(t.toll_Collector_Class);
        bool avcExists = !string.IsNullOrEmpty(t.avC_Class);
        bool tollCorrect = tollExists && t.toll_Collector_Class == t.final_Class;
        bool avcCorrect = avcExists && t.avC_Class == t.final_Class;

        if (tollCorrect && avcCorrect) action = "Both Correct";
        else if (tollCorrect) action = "Toll Collector Correct";
        else if (avcCorrect) action = "AVC Correct";
        else if ((tollExists && !tollCorrect) && (avcExists && !avcCorrect)) action = "Both Incorrect";
        else if (tollExists && !tollCorrect) action = "Toll Collector Incorrect";
        else if (avcExists && !avcCorrect) action = "AVC Incorrect";

        worksheet.Cell(row, 14).Value = action;

        row++;
    }

    using var stream = new MemoryStream();
    workbook.SaveAs(stream);
    stream.Position = 0;
    string fileName = $"DiscrepancyReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
}

}
