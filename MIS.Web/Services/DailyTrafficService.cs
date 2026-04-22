using MIS.Web.Models.Traffic.Daily;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace MIS.Web.Services
{
    public class DailyTrafficReportService : IDailyTrafficReportService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DailyTrafficReportService(
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

            if (classifications?.Any() == true)
            {
                queryParams.Add($"classification={Uri.EscapeDataString(string.Join(",", classifications))}");
            }

            if (operationalDay && shifts?.Any() == true)
            {
                queryParams.Add($"shifts={Uri.EscapeDataString(string.Join(",", shifts))}");
            }

            string baseUrl = _configuration["BaseApiUrl:Link"];
            string endpoint = _configuration["ApiSettings:DailyTrafficEndpoint"];
            string url = $"{CombineUrl(baseUrl, endpoint)}?{string.Join("&", queryParams)}";

            try
            {
                using var request = CreateAuthorizedGetRequest(url);
                using var response = await _httpClient.SendAsync(request);

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
            string url = CombineUrl(baseUrl, endpoint);

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