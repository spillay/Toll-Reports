using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MIS.Web.Models.DailyCashup;

namespace MIS.Web.Services
{
    public class DailyCashupReportService : IDailyCashupReportService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<DailyCashupReportService> _logger;

        public DailyCashupReportService(HttpClient httpClient, IConfiguration config, ILogger<DailyCashupReportService> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
        }

        // 🧾 Fetch main Daily Cashup report
        public async Task<PageDailyCashupModel> GetDailyCashupAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null,
            int page = 1,
            int pageSize = 10)
        {
            string baseUrl = _config["BaseApiUrl:Link"];
            string endpoint = _config["ApiSettings:DailyCashupEndpoint"];

            // Build query string dynamically
            var query = new List<string>
            {
                $"startDate={Uri.EscapeDataString(startDate.ToString("yyyy-MM-ddTHH:mm:ss"))}",
                $"endDate={Uri.EscapeDataString(endDate.ToString("yyyy-MM-ddTHH:mm:ss"))}",
                $"page={page}",
                $"pageSize={pageSize}"
            };

            if (operationalShift?.Count > 0)
                query.Add($"operationalShift={string.Join(",", operationalShift)}");

            if (tollOperators?.Count > 0)
                query.Add($"tollOperators={string.Join(",", tollOperators)}");

            string fullUrl = $"{baseUrl}{endpoint}?{string.Join("&", query)}";

            try
            {
                _logger.LogInformation("➡️ Calling DailyCashup API: {Url}", fullUrl);
                using var response = await _httpClient.GetAsync(fullUrl);

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("⚠️ DailyCashup returned {Code}: {Body}", response.StatusCode, body);
                    return new PageDailyCashupModel();
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<PageDailyCashupModel>(json);

                _logger.LogInformation("✅ DailyCashup: {Count} records received", result?.Items?.Count ?? 0);
                return result ?? new PageDailyCashupModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔥 Failed to call DailyCashup endpoint");
                return new PageDailyCashupModel();
            }
        }

        public async Task<List<string>> GetShiftsAsync()
        {
            var baseUrl = _config["BaseApiUrl:Link"];
            var url = $"{baseUrl}api/DailyCashup/shifts";

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("⚠️ Failed to fetch shifts ({StatusCode})", response.StatusCode);
                    return new List<string>();
                }

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔥 Exception calling /shifts");
                return new List<string>();
            }
        }

        public async Task<List<string>> GetTollOperatorsAsync()
        {
            var baseUrl = _config["BaseApiUrl:Link"];
            var url = $"{baseUrl}api/DailyCashup/operators";

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("⚠️ Failed to fetch operators ({StatusCode})", response.StatusCode);
                    return new List<string>();
                }

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔥 Exception calling /operators");
                return new List<string>();
            }
        }

    }
}
