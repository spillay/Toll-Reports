using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MIS.Web.Services.Interfaces;
using MIS.Web.Models.EndOfDay;

namespace MIS.Web.Pages.Reports
{
    public class EndOfDayModel : PageModel
    {
        private readonly IEndOfDayReportService _service;

        public EndOfDayModel(IEndOfDayReportService service)
        {
            _service = service;
        }

        [BindProperty]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [BindProperty]
        public DateTime EndDate { get; set; } = DateTime.Today;

        public EndOfDayReportViewModel? Report { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Report = await _service.GetEndOfDayAsync(StartDate, EndDate);
            return Page();
        }
    }
}
