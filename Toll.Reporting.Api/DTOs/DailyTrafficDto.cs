namespace Toll.Reporting.Api.DTOs
{
    public class DailyTrafficDto
    {
        public DateTime Date { get; set; }
        public string Classification { get; set; } = "Unknown";
        public int Count { get; set; }
    }
}
