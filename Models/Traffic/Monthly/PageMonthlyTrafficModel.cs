using System.Collections.Generic;

namespace MIS.Web.Models.Traffic.Monthly
{
    public class PageMonthlyTrafficModel
    {
        public MonthlyTrafficInputModel Filters { get; set; } = new();

        public List<MonthlyTrafficModel> Items { get; set; } = new();

        public List<int> AvailableYears { get; set; } = new();
        public List<int> AvailableMonths { get; set; } = new();

        public List<string> AvailableClassifications { get; set; } = new();
    }
}