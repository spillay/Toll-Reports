using MIS.Web.Models.VarientPerfomance;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MIS.Web.Services
{
    public class VarientPerfomanceReportService : IVarientPerfomanceReportService
    {
        private readonly HttpClient _httpClient;

        public VarientPerfomanceReportService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PageVarientPerfomanceModel> GetVarientPerfomanceDetailsAsync(
            int pageNumber,
            int pageSize,
            DateTime startDate,
            DateTime endDate,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null)
        {
            var url = $"http://localhost:5000/api/VarientPerformance/details?" +
          $"startDate={Uri.EscapeDataString(startDate.ToString("s"))}" +
          $"&endDate={Uri.EscapeDataString(endDate.ToString("s"))}" +
          $"&page={pageNumber}&pageSize={pageSize}";

            if (operationalShift != null && operationalShift.Any())
                url += $"&operationalShift={Uri.EscapeDataString(string.Join(",", operationalShift))}";

            if (tollOperators != null && tollOperators.Any())
                url += $"&tollOperators={Uri.EscapeDataString(string.Join(",", tollOperators))}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode) return new PageVarientPerfomanceModel();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PageVarientPerfomanceModel>(json) ?? new PageVarientPerfomanceModel();
        }
    }
}
