using System.Collections.Generic;

namespace MIS.Web.Models.Traffic.Daily
{
    public class PageDailyTrafficModel
    {
        public List<DailyTrafficModel> Items { get; set; } = new List<DailyTrafficModel>();
        // The filter inputs
        public DailyTrafficInputModel Filters { get; set; } = new DailyTrafficInputModel();

        // All classifications available for dropdown
        public List<string> Classifications { get; set; } = new List<string>();
    }
}
