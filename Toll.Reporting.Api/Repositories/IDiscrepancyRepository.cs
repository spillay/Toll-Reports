using Toll.Reporting.Api.DTOs;

public interface IDiscrepancyRepository
{
    Task<IEnumerable<DiscrepancyDto>> GetDiscrepancyAsync(
       DateTime startDate,
       DateTime endDate,
       List<string>? operationalShift = null,
       List<string>? tollOperators = null,
       List<string>? laneNames = null,
       List<string>? paymentMethods = null,
       List<string>? takenAction = null);
}