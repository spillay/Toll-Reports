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
            var queryParts = new List<string>
            {
                $"startDate={Uri.EscapeDataString(startDate.ToString("s"))}",
                $"endDate={Uri.EscapeDataString(endDate.ToString("s"))}",
                $"page={pageNumber}",
                $"pageSize={pageSize}"
            };

            void AddIfAny(string key, List<string>? list)
            {
                if (list == null || !list.Any())
                    return;

                // ✅ Repeat query string keys so API binds List<string> correctly
                foreach (var v in list.Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    queryParts.Add($"{key}={Uri.EscapeDataString(v.Trim())}");
                }
            }

            AddIfAny("operationalShift", operationalShift);
            AddIfAny("tollOperators", tollOperators);

            string baseUrl = _configuration["BaseApiUrl:Link"];
            string endpoint = _configuration["ApiSettings:VarientPerformanceEndpoint"];

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("BaseApiUrl:Link is missing in appsettings.json.");

            if (string.IsNullOrWhiteSpace(endpoint))
                throw new InvalidOperationException("ApiSettings:VarientPerformanceEndpoint is missing in appsettings.json.");

            string url = $"{baseUrl}{endpoint}?{string.Join("&", queryParts)}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new PageVarientPerfomanceModel();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PageVarientPerfomanceModel>(json) ?? new PageVarientPerfomanceModel();
        }

        // =====================================================
        // ✅ NEW: ALL SHIFTS (system-wide)
        // =====================================================
        public async Task<List<string>> GetAllShiftsAsync()
        {
            var url = BuildUrlFromConfig("ApiSettings:VarientPerformanceShiftsEndpoint");

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new List<string>();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();

            return result
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }

        // =====================================================
        // ✅ NEW: ALL OPERATORS (system-wide)
        // =====================================================
        public async Task<List<string>> GetAllTollOperatorsAsync()
        {
            var url = BuildUrlFromConfig("ApiSettings:VarientPerformanceOperatorsEndpoint");

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new List<string>();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();

            return result
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }

        // =====================================================
        // Helper: build full URL from BaseApiUrl + endpoint key
        // =====================================================
        private string BuildUrlFromConfig(string endpointKey)
        {
            string baseUrl = _configuration["BaseApiUrl:Link"];
            string endpoint = _configuration[endpointKey];

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("BaseApiUrl:Link is missing in appsettings.json.");

            if (string.IsNullOrWhiteSpace(endpoint))
                throw new InvalidOperationException($"{endpointKey} is missing in appsettings.json.");

            // Ensure no double slashes issues
            return $"{baseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";
        }
    }
}