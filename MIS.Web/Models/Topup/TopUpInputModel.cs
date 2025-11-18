using MIS.Web.Models.TopUp;
using System;

namespace MIS.Web.Models
{
    public class TopUpInputModel : PageModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string? Operator { get; set; }
        public string? Lane { get; set; }
        public string? Shift { get; set; }
        public string? AccountNumber { get; set; }

        public bool? OperationalDate { get; set; }
        public string? TollOperator { get; set; }
        public PageTopUpModel? PageData { get; set; }
    }
}
