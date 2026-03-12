using Toll.Reporting.Api.DTOs;

public interface IComprehensiveRepository
{
    Task<IEnumerable<ComprehensiveDto>> GetComprehensiveRepositoryAsync(
        DateTime startDate,
        DateTime endDate,
        List<byte>? shiftIds = null,
        List<long>? operatorIds = null,
        List<int>? laneIds = null,
        List<byte>? discountTypeIds = null,
        List<byte>? tollClassIds = null,
        List<byte>? paymentMethodIds = null
    );

    Task<ComprehensiveOptionsDto> GetComprehensiveOptionsAsync();
}