using MIS.Web.Models.Traffic.Monthly;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace MIS.Web.Services
{
    public class MonthlyTrafficReportService : IMonthlyTrafficReportService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MonthlyTrafficReportService(
            HttpClient httpClient,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
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

            var op = operationalMonth ?? false;
            queryParams.Add($"operationalMonth={op.ToString().ToLower()}");

            if (classifications?.Any() == true)
            {
                queryParams.Add($"classification={Uri.EscapeDataString(string.Join(",", classifications))}");
            }

            if (op && shifts?.Any() == true)
            {
                queryParams.Add($"shifts={Uri.EscapeDataString(string.Join(",", shifts))}");
            }

            string baseUrl = _configuration["BaseApiUrl:Link"];
            string endpoint = _configuration["ApiSettings:MonthlyTrafficEndpoint"];
            string url = $"{CombineUrl(baseUrl, endpoint)}?{string.Join("&", queryParams)}";

            try
            {
                using var request = CreateAuthorizedGetRequest(url);
                using var response = await _httpClient.SendAsync(request);

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
            string endpoint = _configuration["ApiSettings:MonthlyTrafficEndpoint"];
            string url = CombineUrl(baseUrl, $"{endpoint}/years");

            try
            {
                using var request = CreateAuthorizedGetRequest(url);
                using var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return new List<int>();

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
                using var request = CreateAuthorizedGetRequest(url);
                using var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return new List<int>();

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
                using var request = CreateAuthorizedGetRequest(url);
                using var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return new List<string>();

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        private HttpRequestMessage CreateAuthorizedGetRequest(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            AddBearerToken(request);
            return request;
        }

        private void AddBearerToken(HttpRequestMessage request)
        {
            var token = _httpContextAccessor.HttpContext?.User?.FindFirst("access_token")?.Value;

            if (string.IsNullOrWhiteSpace(token))
                throw new UnauthorizedAccessException("No JWT token found for current user.");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
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
                AvailableClassifications = new List<string>()
            };
        }
    }
}