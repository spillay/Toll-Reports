using System;

namespace Toll.Reporting.Api.DTOs
{
    public class TopUpDto
    {
        public int TopUpNumber { get; set; }
        public DateTime? TopUpDateTime { get; set; }
        public string? LaneWorkstation { get; set; }
        public string? Shift { get; set; }
        public string? Operator { get; set; }
        public string? AccountNumber { get; set; }
        public string? AccountName { get; set; }
        public decimal AmountPaid { get; set; }
        public string? MethodOfPayment { get; set; }
    }
}
