using MIS.Web.Models.Traffic.Monthly;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MIS.Web.Services
{
    public class MonthlyTrafficReportService : IMonthlyTrafficReportService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public MonthlyTrafficReportService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<PageMonthlyTrafficModel> GetTrafficReportAsync(
            int? year = null,
            int? month = null,
            bool? operationalMonth = null,
            List<string>? classifications = null,
            List<int>? shifts = null)
        {
            // Read base API URL from configuration
            var baseApi = _configuration["ApiSettings:MonthlyTrafficApiUrl"];
            if (string.IsNullOrEmpty(baseApi))
                throw new InvalidOperationException("MonthlyTrafficApiUrl is not configured in appsettings.json.");

            // Build query parameters
            var queryParams = new List<string>();
            if (year.HasValue) queryParams.Add($"year={year.Value}");
            if (month.HasValue) queryParams.Add($"month={month.Value}");
            if (operationalMonth.HasValue) queryParams.Add($"operationalMonth={operationalMonth.Value.ToString().ToLower()}");
            if (classifications?.Any() == true) queryParams.Add($"classification={Uri.EscapeDataString(string.Join(",", classifications))}");
            if (shifts?.Any() == true) queryParams.Add($"shifts={Uri.EscapeDataString(string.Join(",", shifts))}");

            // Combine base URL and query string safely
            var url = baseApi;
            if (queryParams.Any())
                url += "?" + string.Join("&", queryParams);

            // Make HTTP call
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var items = JsonConvert.DeserializeObject<List<MonthlyTrafficModel>>(json) ?? new List<MonthlyTrafficModel>();

            return new PageMonthlyTrafficModel { Items = items };
        }

        // Fetch years for dropdown
        public async Task<List<int>> GetAvailableYearsAsync()
        {
            var baseApi = _configuration["ApiSettings:MonthlyTrafficApiUrl"];
            if (string.IsNullOrEmpty(baseApi))
                throw new InvalidOperationException("MonthlyTrafficApiUrl is not configured in appsettings.json.");

            var response = await _httpClient.GetAsync($"{baseApi}/years");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<int>>(json) ?? new List<int>();
        }

        // Fetch months for dropdown for a given year
        public async Task<List<int>> GetAvailableMonthsAsync(int year)
        {
            var baseApi = _configuration["ApiSettings:MonthlyTrafficApiUrl"];
            if (string.IsNullOrEmpty(baseApi))
                throw new InvalidOperationException("MonthlyTrafficApiUrl is not configured in appsettings.json.");

            var response = await _httpClient.GetAsync($"{baseApi}/months/{year}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<int>>(json) ?? new List<int>();
        }
    }
}
