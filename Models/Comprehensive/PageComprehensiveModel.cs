using System.Collections.Generic;

namespace MIS.Web.Models.Comprehensive
{
    public class PageComprehensiveModel
    {
        public class FilterOption<TId>
        {
            public TId Id { get; set; } = default!;
            public string Name { get; set; } = "";
        }

        public ComprehensiveInputModel Input { get; set; } = new();
        public List<ComprehensiveModel> Items { get; set; } = new();

        public List<FilterOption<byte>> Shifts { get; set; } = new();
        public List<FilterOption<long>> TollOperators { get; set; } = new();
        public List<FilterOption<int>> Lanes { get; set; } = new();
        public List<FilterOption<byte>> PaymentMethods { get; set; } = new();
        public List<FilterOption<byte>> DiscountTypes { get; set; } = new();
        public List<FilterOption<byte>> Classifications { get; set; } = new(); 

        public List<string> TollClasses { get; set; } = new();

        public string FilterTextOperationalShift { get; set; } = "All";
        public string FilterTextOperators { get; set; } = "All";
        public string FilterTextLanes { get; set; } = "All";
        public string FilterTextPaymentMethods { get; set; } = "All";
        public string FilterTextDiscountTypes { get; set; } = "All";
        public string FilterTextClassifications { get; set; } = "All";

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

            public decimal TotalCountPercent { get; set; }
            public decimal TotalRevenuePercent { get; set; }
        }

        public List<GroupedRow> GroupedDataTyped { get; set; } = new();
    }
}