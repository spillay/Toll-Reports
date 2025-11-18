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
            List<string>? operationalShift = null,
            List<string>? tollOperators = null,
            int page = 1,
            int pageSize = 10);

        Task<List<string>> GetShiftsAsync();
        Task<List<string>> GetTollOperatorsAsync();
    }
}
