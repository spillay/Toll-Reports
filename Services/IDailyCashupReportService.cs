using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MIS.Web.Models.DailyCashup;

namespace MIS.Web.Services
{
    public interface IDailyCashupReportService
    {

        Task<PageDailyCashupModel> GetDailyCashupAsync(
            DateTime startDate,
            DateTime endDate,
            List<int>? shiftIds = null,
            List<long>? systemUserIds = null,
            int page = 1,
            int pageSize = 10);

        Task<(List<CheckItemModel<int>> Shifts, List<CheckItemModel<long>> Operators)> GetFiltersAsync();
    }
}