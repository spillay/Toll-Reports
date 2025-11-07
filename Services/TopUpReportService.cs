using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MIS.Web.Models.TopUp;

namespace MIS.Web.Services
{
    public class TopUpReportService : ITopUpReportService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<TopUpReportService> _logger;

        public TopUpReportService(HttpClient httpClient, IConfiguration config, ILogger<TopUpReportService> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
        }

        public async Task<PageTopUpModel> GetTopUpAsync(
            DateTime startDate,
            DateTime endDate,
            string? operatorId = null,
            string? lane = null,
            string? shift = null,
            string? accountNumber = null,
            bool? operationalDate = null,
            int page = 1,
            int pageSize = 50)
        {
            string baseUrl = _config["BaseApiUrl:Link"];
            string endpoint = _config["ApiSettings:TopUpEndpoint"]; // e.g. "api/TopUp/details"

            var query = new List<string>
            {
                $"startDate={Uri.EscapeDataString(startDate.ToString("yyyy-MM-ddTHH:mm:ss"))}",
                $"endDate={Uri.EscapeDataString(endDate.ToString("yyyy-MM-ddTHH:mm:ss"))}",
                $"page={page}",
                $"pageSize={pageSize}"
            };

            if (!string.IsNullOrEmpty(operatorId))
                query.Add($"operatorId={operatorId}");
            if (!string.IsNullOrEmpty(lane))
                query.Add($"lane={lane}");
            if (!string.IsNullOrEmpty(shift))
                query.Add($"shift={shift}");
            if (!string.IsNullOrEmpty(accountNumber))
                query.Add($"accountNumber={accountNumber}");
            if (operationalDate.HasValue)
                query.Add($"operationalDate={operationalDate.Value.ToString().ToLower()}");

            string fullUrl = $"{baseUrl}{endpoint}?{string.Join("&", query)}";

            try
            {
                _logger.LogInformation("➡️ Calling Top-Up API: {Url}", fullUrl);
                using var response = await _httpClient.GetAsync(fullUrl);

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("⚠️ Top-Up API returned {Code}: {Body}", response.StatusCode, body);
                    return new PageTopUpModel();
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<PageTopUpModel>(json);

                _logger.LogInformation("✅ Top-Up Report: {Count} records", result?.items?.Count ?? 0);
                return result ?? new PageTopUpModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔥 Failed to call Top-Up API");
                return new PageTopUpModel();
            }
        }
    }
}
