using System;

namespace MIS.Web.Models.DailyCashup
{
    public class DailyCashupModel
    {
        public DateTime ShiftDate { get; set; }

        public string ShiftDescription { get; set; } = string.Empty;
        public string TollOperator { get; set; } = string.Empty;

        public decimal NettAmount { get; set; }        // Lane Cash
        public decimal ActualAmount { get; set; }      // Top-ups
        public decimal TotalCashExpected { get; set; } // Lane Cash + Top-ups
        public decimal TotalDeclared { get; set; }     // Cash Declared
        public decimal Difference { get; set; }        
        public decimal TotalBanked { get; set; }       

    }
}