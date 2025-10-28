using MIS.Web.Models.VarientPerfomance;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MIS.Web.Services
{
    public class VarientPerfomanceReportService : IVarientPerfomanceReportService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public VarientPerfomanceReportService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<PageVarientPerfomanceModel> GetVarientPerfomanceDetailsAsync(
            int pageNumber,
            int pageSize,
            DateTime startDate,
            DateTime endDate,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null)
        {
            //var baseUrl = _configuration["ApiSettings:VarientPerfomanceApiUrl"];
            //if (string.IsNullOrEmpty(baseUrl))
            //    throw new InvalidOperationException("VarientPerfomanceApiUrl is not configured in appsettings.json.");

            var queryParts = new List<string>
            {
                $"startDate={Uri.EscapeDataString(startDate.ToString("s"))}",
                $"endDate={Uri.EscapeDataString(endDate.ToString("s"))}",
                $"page={pageNumber}",
                $"pageSize={pageSize}"
            };

            void AddIfAny(string key, List<string>? list)
            {
                if (list != null && list.Any())
                    queryParts.Add($"{key}={Uri.EscapeDataString(string.Join(",", list))}");
            }

            AddIfAny("operationalShift", operationalShift);
            AddIfAny("tollOperators", tollOperators);

            string baseUrl = _configuration["BaseApiUrl:Link"];
            string endpoint = _configuration["ApiSettings:VarientPerformanceEndpoint"];
            string url = $"{baseUrl}{endpoint}?{string.Join("&", queryParts)}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new PageVarientPerfomanceModel();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PageVarientPerfomanceModel>(json) ?? new PageVarientPerfomanceModel();
        }
    }
}
