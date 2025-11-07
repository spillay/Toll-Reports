using System.Collections.Generic;

namespace MIS.Web.Models.Traffic.Daily
{
    public class PageDailyTrafficModel
    {
        public List<DailyTrafficModel> Items { get; set; } = new List<DailyTrafficModel>();
        public DailyTrafficInputModel Filters { get; set; } = new DailyTrafficInputModel();

        public List<string> Classifications { get; set; } = new List<string>();
    }
}
