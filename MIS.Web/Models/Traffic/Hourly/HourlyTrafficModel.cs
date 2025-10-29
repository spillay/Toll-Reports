namespace MIS.Web.Models.Traffic.Hourly
{
    public class HourlyTrafficModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Classification { get; set; } = string.Empty;
        public int Count { get; set; }

        public string Hour => StartDate.ToString("HH:mm");
        public string Date => StartDate.ToString("dd/MM/yyyy");
    }

}
