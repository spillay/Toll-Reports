using MIS.Web.Models.Traffic.Daily;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MIS.Web.Services
{
    public interface IDailyTrafficReportService
    {
        Task<PageDailyTrafficModel> GetTrafficReportAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? classifications = null,
            List<int>? shifts = null,
            bool operationalDay = false
        );
    }
}
