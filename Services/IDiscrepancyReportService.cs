using MIS.Web.Models.Discrepancy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MIS.Web.Services
{
    public interface IDiscrepancyReportService
    {
        //  paginated table data using the full model (checklists included)
        Task<PageDiscrepancyModel> GetDiscrepancyReportAsync(DiscrepancyInputModel model);

        //  Backward compatible: old signature still supported (wrapper in service)
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

        //  Filter checklist values (ALL values from DB)
        Task<DiscrepancyInputModel> GetDiscrepancyFilterOptionsAsync(DiscrepancyInputModel model);

        //  Full export (exportAll=true)
        Task<PageDiscrepancyModel> GetFullExportAsync(DiscrepancyInputModel model);
    }
}