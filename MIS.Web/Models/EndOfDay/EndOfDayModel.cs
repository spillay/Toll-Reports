using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MIS.Web.Models.EndOfDay;
using MIS.Web.Services.Interfaces;

namespace MIS.Web.Pages.Reports
{
    public class EndOfDayModel : PageModel
    {
        private readonly IEndOfDayReportService _service;

        public EndOfDayModel(IEndOfDayReportService service)
        {
            _service = service;
        }

        [BindProperty(SupportsGet = true)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [BindProperty(SupportsGet = true)]
        public DateTime EndDate { get; set; } = DateTime.Today;

        [BindProperty(SupportsGet = true)]
        public int? ShiftId { get; set; }

        public EndOfDayReportViewModel Report { get; set; } = new();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Report = await _service.GetEndOfDayAsync(StartDate, EndDate, ShiftId) ?? new EndOfDayReportViewModel();
            return Page();
        }
    }
}