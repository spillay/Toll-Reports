using MIS.Web.Models.AvcAccuracy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MIS.Web.Services.Interfaces
{
    public interface IAvcAccuracyReportService
    {
        Task<PageAvcAccuracyReportModel> GetReportAsync(
            DateTime startDate,
            DateTime endDate,
            List<int>? shiftIds = null,
            List<int>? laneIds = null,
            List<int>? classIds = null);
    }
}