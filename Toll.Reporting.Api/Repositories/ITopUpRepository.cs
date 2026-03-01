using System;
using System.Threading.Tasks;
using Toll.Reporting.Api.DTOs;

namespace Toll.Reporting.Api.Repositories
{
    public interface ITopUpRepository
    {
        Task<PagedResult<TopUpDto>> GetTopUpsAsync(
        DateTime startDate,
        DateTime endDate,
        List<string>? shifts = null,
        List<string>? operatorIds = null,
        List<string>? lanes = null,
        List<string>? paymentMethods = null, 
        string? accountNumber = null,
        int page = 1,
        int pageSize = 30);

        Task<TopUpDto> GetTopUpFilterOptionsAsync();
    }
}
