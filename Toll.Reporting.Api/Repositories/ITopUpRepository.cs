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
            string? operatorId = null,
            string? lane = null,
            string? shift = null,
            string? accountNumber = null,
            bool? operationalDate = null,
            int page = 1,
            int pageSize = 50);
    }
}
