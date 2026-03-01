using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using MIS.Web.Models;
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

        // ✅ Helper: append list values as repeated query params
        private static void AddListParams(List<string> query, string key, List<string>? values)
        {
            if (values == null) return;

            foreach (var v in values.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()))
            {
                query.Add($"{key}={Uri.EscapeDataString(v)}");
            }
        }

        private string BuildUrl(string endpoint, List<string> queryParts)
        {
            var baseUrl = _config["BaseApiUrl:Link"]?.TrimEnd('/') ?? "";
            endpoint = (endpoint ?? "").TrimStart('/');
            return $"{baseUrl}/{endpoint}?{string.Join("&", queryParts)}";
        }

        // =====================================================
        // 0) FILTER OPTIONS (GLOBAL CHECKBOX LISTS)
        // =====================================================
        public async Task<TopUpInputModel> GetTopUpFilterOptionsAsync()
        {
            string endpoint = _config["ApiSettings:TopUpFilterOptionsEndpoint"]
                              ?? "api/TopUp/filter-options";

            var url = BuildUrl(endpoint, new List<string>());

            var resp = await _client.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return new TopUpInputModel();

            var json = await resp.Content.ReadAsStringAsync();

            // The API returns a TopUpDto-like object. We only care about the Options lists.
            // We'll deserialize into TopUpInputModel because we added matching properties there.
            return JsonConvert.DeserializeObject<TopUpInputModel>(json) ?? new TopUpInputModel();
        }

        // =====================================================
        // 1) RAZOR PAGE - PAGED RESULTS
        // =====================================================
        public async Task<PageTopUpModel> GetTopUpAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? shifts = null,
            List<string>? operatorIds = null,
            List<string>? lanes = null,
            List<string>? paymentMethods = null,
            string? accountNumber = null,
            int page = 1,
            int pageSize = 30)
        {
            string endpoint = _config["ApiSettings:TopUpEndpoint"];  // api/TopUp/details

            var query = new List<string>
            {
                $"startDate={Uri.EscapeDataString(startDate.ToString("yyyy-MM-ddTHH:mm:ss"))}",
                $"endDate={Uri.EscapeDataString(endDate.ToString("yyyy-MM-ddTHH:mm:ss"))}",
                $"page={page}",
                $"pageSize={pageSize}"
            };

            //  checkbox lists
            AddListParams(query, "shifts", shifts);
            AddListParams(query, "operatorIds", operatorIds);
            AddListParams(query, "lanes", lanes);
            AddListParams(query, "paymentMethods", paymentMethods);

            if (!string.IsNullOrWhiteSpace(accountNumber))
                query.Add($"accountNumber={Uri.EscapeDataString(accountNumber.Trim())}");

            string url = BuildUrl(endpoint, query);

            var resp = await _client.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return new PageTopUpModel();

            string json = await resp.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PageTopUpModel>(json) ?? new PageTopUpModel();
        }

        // 2) EXPORT MODE - FULL DATASET (NO PAGING)
        public async Task<PageTopUpModel> GetTopUpFullAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? shifts = null,
            List<string>? operatorIds = null,
            List<string>? lanes = null,
            List<string>? paymentMethods = null,
            string? accountNumber = null)
        {
            string endpoint = _config["ApiSettings:TopUpEndpoint"];

            var query = new List<string>
            {
                $"startDate={Uri.EscapeDataString(startDate.ToString("yyyy-MM-ddTHH:mm:ss"))}",
                $"endDate={Uri.EscapeDataString(endDate.ToString("yyyy-MM-ddTHH:mm:ss"))}",
                $"page=1",
                $"pageSize=1000000" 
            };

            // checkbox lists
            AddListParams(query, "shifts", shifts);
            AddListParams(query, "operatorIds", operatorIds);
            AddListParams(query, "lanes", lanes);
            AddListParams(query, "paymentMethods", paymentMethods);

            if (!string.IsNullOrWhiteSpace(accountNumber))
                query.Add($"accountNumber={Uri.EscapeDataString(accountNumber.Trim())}");

            string url = BuildUrl(endpoint, query);

            var resp = await _client.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return new PageTopUpModel();

            string json = await resp.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PageTopUpModel>(json) ?? new PageTopUpModel();
        }
    }
}