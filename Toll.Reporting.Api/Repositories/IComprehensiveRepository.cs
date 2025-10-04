using Toll.Reporting.Api.DTOs;

public interface IComprehensiveRepository
{
    Task<IEnumerable<ComprehensiveDto>> GetComprehensiveRepositoryAsync1(
        DateTime startDate,
        DateTime endDate,
        List<string>? operationalShift = null,
        List<string>? tollOperators = null,
        List<string>? laneNames = null ,
        List<string>? laneDiscountType = null ,
        List<string>? Classification = null ,
        List<string>? paymentMethods = null);
    Task<IEnumerable<ComprehensiveDto>> GetComprehensiveRepositoryAsync(DateTime startDate, DateTime endDate, string paymentMethods);
}
