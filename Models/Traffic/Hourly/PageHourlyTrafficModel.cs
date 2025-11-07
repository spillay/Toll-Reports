namespace MIS.Web.Models.Traffic.Hourly
{
    public class PageHourlyTrafficModel
    {
        // Table data
        public List<HourlyTrafficModel> Items { get; set; } = new List<HourlyTrafficModel>();

        // Filter input
        public HourlyTrafficInputModel Input { get; set; } = new HourlyTrafficInputModel();
    }
}
