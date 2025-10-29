using System;

namespace MIS.Web.Models.Discrepancy
{
    public class DiscrepancyInputModel
    {
        public DateTime StartDate { get; set; } = DateTime.Now.AddDays(-90);
        public DateTime EndDate { get; set; } = DateTime.Now;

        public string? lane_Nr { get; set; }
        public string? Shift { get; set; }
        public string? PaymentMethod { get; set; }
        public string? TakenAction { get; set; }
        public string? toll_Operator_ID { get; set; } 

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
