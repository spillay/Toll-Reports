using System;
using System.Collections.Generic;

namespace MIS.Web.Pages.Shared
{
    public class TransactionFiltersModel
    {
        public DateTime StartDate { get; set; } = DateTime.Now.AddDays(-7);
        public DateTime EndDate { get; set; } = DateTime.Now;
        public List<string>? PaymentMethods { get; set; } = new List<string>();
    }
}
