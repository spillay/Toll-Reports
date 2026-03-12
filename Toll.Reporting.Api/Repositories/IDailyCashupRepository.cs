using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toll.Reporting.Api.DTOs;

namespace Toll.Reporting.Api.Repositories
{
    public interface IDailyCashupRepository
    {
        Task<PagedResult<DailyCashupDto>> GetDailyCashupAsync(
        DateTime startDate,
        DateTime endDate,
        List<int>? shiftIds = null,
        List<long>? systemUserIds = null,
        int page = 1,
        int pageSize = 10);

        Task<DailyCashupFilterOptionsDto> GetDailyCashupFilterOptionsAsync();

    }
}