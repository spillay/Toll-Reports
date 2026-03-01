using MIS.Web.Models.Traffic.Daily;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MIS.Web.Services
{
    public class DailyTrafficReportService : IDailyTrafficReportService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public DailyTrafficReportService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }
        private static string CombineUrl(string baseUrl, string endpoint)
        {
            baseUrl = (baseUrl ?? "").TrimEnd('/');
            endpoint = (endpoint ?? "").TrimStart('/');
            return $"{baseUrl}/{endpoint}";
        }
        public async Task<PageDailyTrafficModel> GetTrafficReportAsync(
    DateTime startDate,
    DateTime endDate,
    List<string>? classifications = null,
    List<int>? shifts = null,
    bool operationalDay = false)
        {
            var queryParams = new List<string>
    {
        $"startDate={Uri.EscapeDataString(startDate.ToString("yyyy-MM-dd"))}",
        $"endDate={Uri.EscapeDataString(endDate.ToString("yyyy-MM-dd"))}",
        $"operationalDay={operationalDay.ToString().ToLower()}"
    };

            // ✅ send multiple classes as CSV
            if (classifications?.Any() == true)
            {
                queryParams.Add($"classification={Uri.EscapeDataString(string.Join(",", classifications))}");
            }

            if (operationalDay && shifts?.Any() == true)
            {
                queryParams.Add($"shifts={Uri.EscapeDataString(string.Join(",", shifts))}");
            }

            string baseUrl = _configuration["BaseApiUrl:Link"];
            string endpoint = _configuration["ApiSettings:DailyTrafficEndpoint"]; // e.g. /api/DailyTraffic/GetDailyTrafficByDate
            string url = $"{CombineUrl(baseUrl, endpoint)}?{string.Join("&", queryParams)}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return CreateEmptyModel(startDate, endDate, classifications, shifts, operationalDay);

                var json = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<DailyTrafficModel>>(json) ?? new List<DailyTrafficModel>();

                return new PageDailyTrafficModel
                {
                    Items = items,
                    Filters = new DailyTrafficInputModel
                    {
                        StartDate = startDate,
                        EndDate = endDate,
                        Classification = classifications != null ? string.Join(",", classifications) : null,
                        Shifts = shifts ?? new List<int>(),
                        OperationalDay = operationalDay
                    },

                    // ✅ DO NOT set Classifications here anymore
                    // Controller will set it using GetAllClassificationsAsync()
                    Classifications = new List<string>()
                };
            }
            catch
            {
                return CreateEmptyModel(startDate, endDate, classifications, shifts, operationalDay);
            }
        }

        public async Task<List<string>> GetAllClassificationsAsync()
        {
            string baseUrl = _configuration["BaseApiUrl:Link"];
            string endpoint = _configuration["ApiSettings:DailyTrafficClassificationsEndpoint"];
            // e.g. /api/DailyTraffic/GetAllClassifications

            string url = CombineUrl(baseUrl, endpoint);

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return new List<string>();

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        private PageDailyTrafficModel CreateEmptyModel(
            DateTime startDate,
            DateTime endDate,
            List<string>? classifications,
            List<int>? shifts,
            bool operationalDay)
        {
            return new PageDailyTrafficModel
            {
                Items = new List<DailyTrafficModel>(),
                Filters = new DailyTrafficInputModel
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Classification = classifications?.FirstOrDefault(),
                    Shifts = shifts ?? new List<int>(),
                    OperationalDay = operationalDay
                },
                Classifications = new List<string> { "Class 1", "Class 2", "Class 4", "Class M" }
            };
        }
    }
}
