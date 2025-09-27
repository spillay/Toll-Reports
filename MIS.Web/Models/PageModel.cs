using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MIS.Web.Models;
using MIS.Web.Services;

public class TransactionReportModel : PageModel
{
    private readonly IReportService _reportService;

    public TransactionReportModel(IReportService reportService)
    {
        _reportService = reportService;
    }

    [BindProperty(SupportsGet = true)]
    public DateTime StartDate { get; set; } = DateTime.Now.AddDays(-7);

    [BindProperty(SupportsGet = true)]
    public DateTime EndDate { get; set; } = DateTime.Now;

    public List<TransactionReportViewModel> Transactions { get; set; } = new();

    public async Task OnGetAsync()
    {
        Transactions = await _reportService.GetTransactionDetailsAsync(StartDate, EndDate);
    }
}
