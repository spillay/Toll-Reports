using System.Buffers.Text;
using System.Linq;

namespace MIS.Web.Services
{
    public class ReportService : IReportService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly String baseURL = "http://localhost:5142/";

        public ReportService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetShiftsAsync()
        {
            var url = baseURL + "api/Lookup/shifts";
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(baseURL);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
