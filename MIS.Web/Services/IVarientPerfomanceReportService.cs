using MIS.Web.Models.VarientPerfomance;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MIS.Web.Services
{
    public interface IVarientPerfomanceReportService
    {
        Task<PageVarientPerfomanceModel> GetVarientPerfomanceDetailsAsync(
        int pageNumber,
        int pageSize,
        DateTime startDate,
        DateTime endDate,
        List<string>? operationalShift = null,
        List<string>? tollOperators = null);

    }
}
