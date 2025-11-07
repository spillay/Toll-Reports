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
        public string? TransactionType { get; set; }
        public string? DiscountType { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public string? Shift { get; set; }
        public string? ManualTollClass { get; set; }
        public int TariffPlanId { get; set; }
  
        public double AmountInclusive { get; set; }

        public string? MethodOfPayment { get; set; }
      
        public byte DiscountTypeId { get; set; }
        public long? SystemUserId { get; set; }
        public byte ManualTollClassId { get; set; }
        public int TariffPlanDetailId { get; set; }
    }
}
