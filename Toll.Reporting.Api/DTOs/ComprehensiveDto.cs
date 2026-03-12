namespace Toll.Reporting.Api.DTOs
{
    public class ComprehensiveDto
    {
        public string? LaneName { get; set; }
        public string? DiscountType { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public string? ShiftName { get; set; }
        public string? ManualTollClassName { get; set; }
        public int? TariffPlanId { get; set; }
        public double? AmountInclusive { get; set; }

        public int? PaymentMethodId { get; set; }
        public string? PaymentMethodName { get; set; }

        public long? TollOperatorId { get; set; }
        public string? TollOperatorName { get; set; }

        public int? LaneId { get; set; }
        public int? DiscountTypeId { get; set; }
        public int? ShiftId { get; set; }
        public int? ManualTollClassId { get; set; }
    }

    public class FilterOptionDto<T>
    {
        public T Id { get; set; } = default!;
        public string Name { get; set; } = "";
    }

    public class ComprehensiveOptionsDto
    {
        public List<FilterOptionDto<byte>> Shifts { get; set; } = new();
        public List<FilterOptionDto<long>> Operators { get; set; } = new();
        public List<FilterOptionDto<int>> Lanes { get; set; } = new();
        public List<FilterOptionDto<byte>> DiscountTypes { get; set; } = new();
        public List<FilterOptionDto<byte>> TollClasses { get; set; } = new();

        public List<FilterOptionDto<byte>> PaymentMethods { get; set; } = new();
    }
}