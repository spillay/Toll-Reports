namespace MIS.Web.Models.DailyCashup
{
    public class DailyCashupModel
    {
        public DateTime ShiftDate { get; set; }
        public string? ShiftDescription { get; set; }
        public string? TollOperator { get; set; }
        public double NettAmount { get; set; }
        public double ActualAmount { get; set; }
        public double TotalDeclared { get; set; }
        public double Difference { get; set; }
    }
}
