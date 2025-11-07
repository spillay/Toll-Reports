namespace MIS.Web.Models.Traffic.Hourly
{
    public class HourlyTrafficInputModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }  // New property
        public string? Classification { get; set; }
        public List<int>? Shifts { get; set; } = new List<int>();
        public bool OperationalDay { get; set; } // New property
    }
}
