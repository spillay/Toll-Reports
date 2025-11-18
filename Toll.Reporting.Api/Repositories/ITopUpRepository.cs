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
            string? shift = null,
            string? operatorId = null,
            string? lane = null,
            string? accountNumber = null,
            int page = 1,
            int pageSize = 30);
    }
}
