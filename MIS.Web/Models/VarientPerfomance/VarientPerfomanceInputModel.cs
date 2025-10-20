using MIS.Web.Models.VarientPerfomance;

namespace MIS.Web.Models
{
    public class VarientPerfomanceInputModel : PageVarientPerfomanceModel
    {
        public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-30);
        public DateTime EndDate { get; set; } = DateTime.Today;
        public string? Shift { get; set; }
        public string? TollOperatorID { get; set; }
    }
}
