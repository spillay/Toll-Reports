using Toll.Reporting.Api.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Toll.Reporting.Api.Repositories
{
    public interface IAccountRegistrationDetailsRepository
    {
        Task<PagedResult<AccountRegistrationDetailsDto>> GetAccountRegistrationDetailsAsync(
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? status = null,
            bool? isActive = null,
            int page = 1,
            int pageSize = 50);
    }
}
