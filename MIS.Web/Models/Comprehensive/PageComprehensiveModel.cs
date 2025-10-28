using System;
using System.Collections.Generic;

namespace MIS.Web.Models.Comprehensive
{
    public class PageComprehensiveModel
    {
        // Filters (bound to the form in the view)
        public ComprehensiveInputModel Input { get; set; } = new();

        // Raw items (from service) used for totals and reference
        public List<ComprehensiveModel> Items { get; set; } = new();

        // Dropdown lists
        public List<string> TollClasses { get; set; } = new();
        public List<string> Shifts { get; set; } = new();
        public List<string> TransactionTypes { get; set; } = new();
        public List<string> TollOperators { get; set; } = new();
        public List<string> Lanes { get; set; } = new();
        public List<string> PaymentMethods { get; set; } = new();
        public List<string> DiscountTypes { get; set; } = new();
        public List<string> Classifications { get; set; } = new();

        // Typed grouping types (same as your PageModel)
        public class ClassMetrics
        {
            public int Count { get; set; }
            public decimal CountPercent { get; set; }
            public double Revenue { get; set; }
            public decimal RevenuePercent { get; set; }
        }

        public class GroupedRow
        {
            public string Method { get; set; } = string.Empty;
            public Dictionary<string, ClassMetrics> Classes { get; set; } = new();
            public int TotalCount { get; set; }
            public double TotalRevenue { get; set; }
        }

        public List<GroupedRow> GroupedDataTyped { get; set; } = new();
    }
}
