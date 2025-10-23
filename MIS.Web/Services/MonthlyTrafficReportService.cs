using MIS.Web.Models.Traffic.Monthly;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MIS.Web.Services
{
    public class MonthlyTrafficReportService : IMonthlyTrafficReportService
    {
        private readonly HttpClient _httpClient;
        // Base API path - keep in sync with your API host/port
        private const string BaseApi = "http://localhost:5000/api/MonthlyTraffic";

        public MonthlyTrafficReportService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Main report call
        public async Task<PageMonthlyTrafficModel> GetTrafficReportAsync(
            int? year = null,
            int? month = null,
            bool? operationalMonth = null,
            List<string>? classifications = null,
            List<int>? shifts = null)
        {
            var query = new List<string>();

            if (year.HasValue) query.Add($"year={year.Value}");
            if (month.HasValue) query.Add($"month={month.Value}");
            if (operationalMonth.HasValue) query.Add($"operationalMonth={operationalMonth.Value.ToString().ToLower()}");
            if (classifications != null && classifications.Any()) query.Add($"classification={Uri.EscapeDataString(string.Join(",", classifications))}");
            if (shifts != null && shifts.Any()) query.Add($"shifts={string.Join(",", shifts)}");

            var url = BaseApi;
            if (query.Any()) url += "?" + string.Join("&", query);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var items = JsonConvert.DeserializeObject<List<MonthlyTrafficModel>>(json) ?? new List<MonthlyTrafficModel>();

            return new PageMonthlyTrafficModel { Items = items };
        }

        // Fetch years for dropdown
        public async Task<List<int>> GetAvailableYearsAsync()
        {
            var response = await _httpClient.GetAsync($"{BaseApi}/years");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var years = JsonConvert.DeserializeObject<List<int>>(json) ?? new List<int>();
            return years;
        }

        // Fetch months for dropdown for a given year
        public async Task<List<int>> GetAvailableMonthsAsync(int year)
        {
            var response = await _httpClient.GetAsync($"{BaseApi}/months/{year}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var months = JsonConvert.DeserializeObject<List<int>>(json) ?? new List<int>();
            return months;
        }
    }
}
