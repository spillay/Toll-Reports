using System;

namespace MIS.Web.Models.AccountUsageSummary
{
    public class AccountUsageSummaryInputModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string? AccountNumber { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}