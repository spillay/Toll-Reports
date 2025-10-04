using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MIS.Web.Models.Discrepancy;
using MIS.Web.Services;

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
}
