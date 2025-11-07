using System.Collections.Generic;

namespace MIS.Web.Models.Traffic.Monthly
{
    public class PageMonthlyTrafficModel
    {
        public MonthlyTrafficInputModel Filters { get; set; } = new MonthlyTrafficInputModel();
        public List<MonthlyTrafficModel> Items { get; set; } = new List<MonthlyTrafficModel>();
        public List<string> Classifications { get; set; } = new List<string>();
    }
}
