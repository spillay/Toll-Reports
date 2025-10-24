using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace Toll.Reporting.Api.DTOs
{
    public class ComprehensiveDto
    {
        /*
         * DTO represents a single output row for the Comprehensive report.
         * Properties are populated inside the repository by looking up related entities.
         */

        // primary (populated)
        public string? LaneName { get; set; }
       // public long TransactionNumber { get; set; }
        public string? TransactionType { get; set; }
        public string? DiscountType { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public string? Shift { get; set; }
      //  public DateTime ShiftDate { get; set; }
        //public string? Username { get; set; }
        public string? ManualTollClass { get; set; }
        public int TariffPlanId { get; set; }
       // public DateTime EffectiveDate { get; set; }
      //  public int CurrencyId { get; set; }
        public double AmountInclusive { get; set; }

        // Additional metadata (optional)
       // public byte LaneId { get; set; }
     //   public byte TransactionTypeId { get; set; }
        public string? MethodOfPayment { get; set; }
        //public DateTime StartDate { get; set; }
        //public DateTime EndDate { get; set; }

        // other fields retained if needed for advanced filtering
        public byte DiscountTypeId { get; set; }
       // public byte ShiftId { get; set; }
        public long? SystemUserId { get; set; }
        public byte ManualTollClassId { get; set; }
        public int TariffPlanDetailId { get; set; }
    }
}
