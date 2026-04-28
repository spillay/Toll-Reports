using System.Collections.Generic;
using System.Threading.Tasks;

namespace MIS.Web.Services
{
    public interface IApiClientService
    {
        string BuildUrl(string endpointConfigKey, IEnumerable<string>? queryParts = null);
        Task<T?> GetAsync<T>(string endpointConfigKey, IEnumerable<string>? queryParts = null);
    }
}
