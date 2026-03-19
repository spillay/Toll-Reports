using System.Collections.Generic;
using System.Threading.Tasks;
using Toll.Reporting.Api.Models.AvcAccuracy;

namespace Toll.Reporting.Api.Repositories
{
    public interface IAvcAccuracyRepository
    {
        Task<List<AvcAccuracyBaseRow>> GetBaseDataAsync(AvcAccuracyRequest request);
        Task<AvcAccuracyFilterOptionsResponse> GetFilterOptionsAsync();
    }
}