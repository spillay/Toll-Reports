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
            _httpClient = httpClient;
            _configuration = configuration;
        }

        private static string CombineUrl(string baseUrl, string endpoint)
        {
            baseUrl = (baseUrl ?? "").TrimEnd('/');
            endpoint = (endpoint ?? "").TrimStart('/');
            return $"{baseUrl}/{endpoint}";
        }

        public async Task<PageMonthlyTrafficModel> GetTrafficReportAsync(
            int? year = null,
            int? month = null,
            bool? operationalMonth = null,
            List<string>? classifications = null,
            List<int>? shifts = null)
        {
            var queryParams = new List<string>();

            if (year.HasValue) queryParams.Add($"year={year.Value}");
            if (month.HasValue) queryParams.Add($"month={month.Value}");

            // Always send it (keeps API behavior consistent)
            var op = (operationalMonth ?? false);
            queryParams.Add($"operationalMonth={op.ToString().ToLower()}");

            if (classifications?.Any() == true)
            {
                queryParams.Add($"classification={Uri.EscapeDataString(string.Join(",", classifications))}");
            }

            //  Only include shifts when operational month is on
            if (op && shifts?.Any() == true)
            {
                queryParams.Add($"shifts={Uri.EscapeDataString(string.Join(",", shifts))}");
            }

            string baseUrl = _configuration["BaseApiUrl:Link"];
            string endpoint = _configuration["ApiSettings:MonthlyTrafficEndpoint"]; // "api/MonthlyTraffic"
            string url = $"{CombineUrl(baseUrl, endpoint)}?{string.Join("&", queryParams)}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return CreateEmptyModel(year, month, op, classifications, shifts);

                var json = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<MonthlyTrafficModel>>(json) ?? new List<MonthlyTrafficModel>();

                return new PageMonthlyTrafficModel
                {
                    Items = items,
                    Filters = new MonthlyTrafficInputModel
                    {
                        Year = year,
                        Month = month,
                        OperationalMonth = op,
                        Classifications = classifications ?? new List<string>(),
                        Shifts = shifts ?? new List<int>()
                    }
                };
            }
            catch
            {
                return CreateEmptyModel(year, month, op, classifications, shifts);
            }
        }

        public async Task<List<int>> GetAvailableYearsAsync()
        {
            string baseUrl = _configuration["BaseApiUrl:Link"];
            string endpoint = _configuration["ApiSettings:MonthlyTrafficEndpoint"]; // "api/MonthlyTraffic"
            string url = CombineUrl(baseUrl, $"{endpoint}/years");

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return new List<int>();

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<int>>(json) ?? new List<int>();
            }
            catch
            {
                return new List<int>();
            }
        }

        public async Task<List<int>> GetAvailableMonthsAsync(int year)
        {
            string baseUrl = _configuration["BaseApiUrl:Link"];
            string endpoint = _configuration["ApiSettings:MonthlyTrafficEndpoint"];
            string url = CombineUrl(baseUrl, $"{endpoint}/months/{year}");

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return new List<int>();

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<int>>(json) ?? new List<int>();
            }
            catch
            {
                return new List<int>();
            }
        }

        public async Task<List<string>> GetAvailableClassificationsAsync()
        {
            string baseUrl = _configuration["BaseApiUrl:Link"];
            string endpoint = _configuration["ApiSettings:MonthlyTrafficEndpoint"];
            string url = CombineUrl(baseUrl, $"{endpoint}/classifications");

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

        private PageMonthlyTrafficModel CreateEmptyModel(
            int? year,
            int? month,
            bool operationalMonth,
            List<string>? classifications,
            List<int>? shifts)
        {
            return new PageMonthlyTrafficModel
            {
                Items = new List<MonthlyTrafficModel>(),
                Filters = new MonthlyTrafficInputModel
                {
                    Year = year,
                    Month = month,
                    OperationalMonth = operationalMonth,
                    Classifications = classifications ?? new List<string>(),
                    Shifts = shifts ?? new List<int>()
                },
                AvailableClassifications = new List<string>() // controller/view can still populate
            };
        }
    }
}