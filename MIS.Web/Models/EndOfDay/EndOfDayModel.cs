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
        public DateTime? ReportDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? ShiftId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public EndOfDayReportViewModel Report { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var selectedDate = ReportDate?.Date ?? DateTime.Today;

            StartDate = selectedDate.AddHours(5).AddMinutes(30);
            EndDate = selectedDate.AddDays(1).AddHours(5).AddMinutes(29);

            Report = await _service.GetEndOfDayAsync(StartDate, EndDate, ShiftId)
                     ?? new EndOfDayReportViewModel();

            return Page();
        }
    }
}