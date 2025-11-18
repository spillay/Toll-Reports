namespace MIS.Web.Models.DailyCashup
{
    public class DailyCashupModel
    {
        public DateTime ShiftDate { get; set; }
        public string ShiftDescription { get; set; }
        public string TollOperator { get; set; }

        public double NettAmount { get; set; }      // Lane Cash
        public double ActualAmount { get; set; }    // Top-ups

        public double TotalCashExpected => NettAmount + ActualAmount;

        public double TotalDeclared { get; set; }   // Cash Declared

        public double SurplusShortage => TotalCashExpected - TotalDeclared;

        public double TotalBanked => TotalDeclared;
    }
}
