using System;

namespace MIS.Web.Models.AccountUsageDetails
{
    public class AccountUsageDetailsInputModel
    {
        public string? AccountNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}