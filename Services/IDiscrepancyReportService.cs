using MIS.Web.Models.Discrepancy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MIS.Web.Services
{
    public interface IDiscrepancyReportService
    {
        Task<PageDiscrepancyModel> GetDiscrepancyReportAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null,
            List<string>? laneNames = null,
            List<string>? paymentMethods = null,
            List<string>? takenAction = null,
            int page = 1,
            int pageSize = 50);
    }
}
