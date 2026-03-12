using System;

namespace MIS.Web.Models.Comprehensive
{
    public class ComprehensiveModel
    {
        public DateTime TransactionDateTime { get; set; }

        public byte? ShiftId { get; set; }
        public string? ShiftName { get; set; }

        public long? TollOperatorId { get; set; }
        public string? TollOperatorName { get; set; }

        public int? LaneId { get; set; }
        public string? LaneName { get; set; }

        public byte? DiscountTypeId { get; set; }
        public string? DiscountTypeName { get; set; }

        public byte? ManualTollClassId { get; set; }
        public string? ManualTollClassName { get; set; }

        public byte? PaymentMethodId { get; set; }
        public string? PaymentMethodName { get; set; }

        public int? TariffPlanId { get; set; }
        public double? AmountInclusive { get; set; }
    }
}