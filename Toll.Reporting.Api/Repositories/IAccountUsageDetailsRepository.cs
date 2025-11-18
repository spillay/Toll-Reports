using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toll.Reporting.Api.DTOs;

namespace Toll.Reporting.Api.Repositories.Interfaces
{
    public interface IAccountUsageDetailsRepository
    {
        Task<AccountUsageDetailsReportDto> GetAccountUsageDetailsAsync(DateTime startDate, DateTime endDate);
    }
}
