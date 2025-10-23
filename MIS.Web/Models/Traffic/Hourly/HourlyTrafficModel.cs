namespace MIS.Web.Models.Traffic.Hourly
{
    public class HourlyTrafficModel
    {
        public DateTime Period { get; set; } 
        public string? Classification { get; set; }
        public int Count { get; set; }
    }
}
