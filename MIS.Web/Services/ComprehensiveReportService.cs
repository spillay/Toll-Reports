using MIS.Web.Models.Comprehensive;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MIS.Web.Services
{
    public class ComprehensiveReportService : IComprehensiveReportService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ComprehensiveReportService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        private static string CombineUrl(string baseUrl, string endpoint)
        {
            baseUrl = (baseUrl ?? "").TrimEnd('/');
            endpoint = (endpoint ?? "").TrimStart('/');
            return $"{baseUrl}/{endpoint}";
        }

        public async Task<ComprehensiveOptionsResponse> GetComprehensiveOptionsAsync()
        {
            var baseUrl = _configuration["BaseApiUrl:Link"];
            var endpoint = _configuration["ApiSettings:ComprehensiveOptionsEndpoint"]; 
            var url = CombineUrl(baseUrl, endpoint);

            var resp = await _httpClient.GetAsync(url);
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ComprehensiveOptionsResponse>(json) ?? new ComprehensiveOptionsResponse();
        }

        public async Task<List<ComprehensiveModel>> GetComprehensiveDetailsAsync(
            DateTime startDate,
            DateTime endDate,
            List<byte>? shiftIds = null,
            List<long>? operatorIds = null,
            List<int>? laneIds = null,
            List<byte>? discountTypeIds = null,
            List<byte>? tollClassIds = null,
            List<byte>? paymentMethodIds = null)
        {
            var baseUrl = _configuration["BaseApiUrl:Link"];
            var endpoint = _configuration["ApiSettings:ComprehensiveReportEndpoint"];
            var url = CombineUrl(baseUrl, endpoint);

            var query = new List<string>
            {
                $"startDate={Uri.EscapeDataString(startDate.ToString("yyyy-MM-dd"))}",
                $"endDate={Uri.EscapeDataString(endDate.ToString("yyyy-MM-dd"))}"
            };

            static void AddList<T>(List<string> q, string key, IEnumerable<T>? list)
            {
                if (list == null) return;
                foreach (var v in list)
                    q.Add($"{key}={Uri.EscapeDataString(Convert.ToString(v) ?? "")}");
            }

            AddList(query, "shiftIds", shiftIds);
            AddList(query, "operatorIds", operatorIds);
            AddList(query, "laneIds", laneIds);
            AddList(query, "discountTypeIds", discountTypeIds);
            AddList(query, "tollClassIds", tollClassIds);
            AddList(query, "paymentMethodIds", paymentMethodIds);

            var fullUrl = $"{url}?{string.Join("&", query)}";

            var resp = await _httpClient.GetAsync(fullUrl);
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<ComprehensiveModel>>(json) ?? new List<ComprehensiveModel>();
        }
    }
}