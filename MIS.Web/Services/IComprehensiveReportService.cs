using MIS.Web.Models.Comprehensive;
using MIS.Web.Models.Discrepancy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MIS.Web.Services
{
    public interface IComprehensiveReportService
    {
            Task<List<ComprehensiveReportViewModel>> GetComprehensiveDetailsAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null,
            List<string>? laneNames = null,
            List<string>? TransactionType = null);
    }
}
