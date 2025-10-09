using MIS.Web.Models.Comprehensive;
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
            List<string>? paymentMethods = null,
            List<string>? laneDiscountTypes = null,
            List<string>? classification = null,
            List<string>? transactionTypes = null);
    }
}
