using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using MIS.Web.Models.TopUp;

namespace MIS.Web.Services
{
    public class TopUpReportService : ITopUpReportService
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _config;

        public TopUpReportService(HttpClient client, IConfiguration config)
        {
            _client = client;
            _config = config;
        }

        // =====================================================
        // 1) RAZOR PAGE - PAGED RESULTS
        // =====================================================
        public async Task<PageTopUpModel> GetTopUpAsync(
            DateTime startDate,
            DateTime endDate,
            string? shift = null,
            string? operatorId = null,
            string? lane = null,
            string? accountNumber = null,
            int page = 1,
            int pageSize = 30)
        {
            string baseUrl = _config["BaseApiUrl:Link"];
            string endpoint = _config["ApiSettings:TopUpEndpoint"];  // api/TopUp/details

            var query = new List<string>
            {
                $"startDate={startDate:yyyy-MM-ddTHH:mm:ss}",
                $"endDate={endDate:yyyy-MM-ddTHH:mm:ss}",
                $"page={page}",
                $"pageSize={pageSize}"
            };

            if (!string.IsNullOrWhiteSpace(shift))
                query.Add($"shift={Uri.EscapeDataString(shift)}");

            if (!string.IsNullOrWhiteSpace(operatorId))
                query.Add($"operatorId={Uri.EscapeDataString(operatorId)}");

            if (!string.IsNullOrWhiteSpace(lane))
                query.Add($"lane={Uri.EscapeDataString(lane)}");

            if (!string.IsNullOrWhiteSpace(accountNumber))
                query.Add($"accountNumber={Uri.EscapeDataString(accountNumber)}");

            string url = $"{baseUrl}{endpoint}?{string.Join("&", query)}";

            var resp = await _client.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return new PageTopUpModel();

            string json = await resp.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PageTopUpModel>(json)
                   ?? new PageTopUpModel();
        }

        // =====================================================
        // 2) EXPORT MODE - FULL DATASET (NO PAGING)
        // =====================================================
        public async Task<PageTopUpModel> GetTopUpFullAsync(
            DateTime startDate,
            DateTime endDate,
            string? shift = null,
            string? operatorId = null,
            string? lane = null,
            string? accountNumber = null)
        {
            string baseUrl = _config["BaseApiUrl:Link"];
            string endpoint = _config["ApiSettings:TopUpEndpoint"];

            // ⭐ Set pageSize very high to fetch ALL rows
            var query = new List<string>
            {
                $"startDate={startDate:yyyy-MM-ddTHH:mm:ss}",
                $"endDate={endDate:yyyy-MM-ddTHH:mm:ss}",
                $"page=1",
                $"pageSize=1000000"
            };

            if (!string.IsNullOrWhiteSpace(shift))
                query.Add($"shift={Uri.EscapeDataString(shift)}");

            if (!string.IsNullOrWhiteSpace(operatorId))
                query.Add($"operatorId={Uri.EscapeDataString(operatorId)}");

            if (!string.IsNullOrWhiteSpace(lane))
                query.Add($"lane={Uri.EscapeDataString(lane)}");

            if (!string.IsNullOrWhiteSpace(accountNumber))
                query.Add($"accountNumber={Uri.EscapeDataString(accountNumber)}");

            string url = $"{baseUrl}{endpoint}?{string.Join("&", query)}";

            var resp = await _client.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return new PageTopUpModel();

            string json = await resp.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PageTopUpModel>(json)
                   ?? new PageTopUpModel();
        }
    }
}
