using MIS.Web.Models.Traffic.Hourly;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace MIS.Web.Services
{
    public class HourlyTrafficReportService : IHourlyTrafficReportService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HourlyTrafficReportService(
            HttpClient httpClient,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<List<string>> GetAllClassificationsAsync()
        {
            try
            {
                var baseUrl = (_configuration["BaseApiUrl:Link"] ?? string.Empty).TrimEnd('/');
                var url = $"{baseUrl}/api/HourlyTraffic/GetAllClassifications";

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

        public async Task<PageHourlyTrafficModel> GetTrafficReportAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? classifications = null,
            List<int>? shifts = null,
            bool operationalDay = false)
        {
            try
            {
                var queryParams = new List<string>
                {
                    $"startDate={Uri.EscapeDataString(startDate.ToString("MM/dd/yyyy"))}",
                    $"endDate={Uri.EscapeDataString(endDate.ToString("MM/dd/yyyy"))}",
                    $"operationalDay={operationalDay.ToString().ToLower()}"
                };

                if (classifications?.Any() == true)
                    queryParams.Add($"classification={Uri.EscapeDataString(string.Join(",", classifications))}");

                if (shifts?.Any() == true)
                    queryParams.Add($"shifts={Uri.EscapeDataString(string.Join(",", shifts))}");

                var baseUrl = (_configuration["BaseApiUrl:Link"] ?? string.Empty).TrimEnd('/');
                var endpoint = (_configuration["ApiSettings:HourlyTrafficEndpoint"] ?? string.Empty).TrimStart('/');

                var url = $"{baseUrl}/{endpoint}?{string.Join("&", queryParams)}";

                using var request = CreateAuthorizedGetRequest(url);
                using var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return new PageHourlyTrafficModel();

                var json = await response.Content.ReadAsStringAsync();
                var apiResult = JsonConvert.DeserializeObject<List<HourlyTrafficModel>>(json);

                return new PageHourlyTrafficModel
                {
                    Items = apiResult?.Select(x => new HourlyTrafficModel
                    {
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        Classification = x.Classification,
                        Count = x.Count
                    }).ToList() ?? new List<HourlyTrafficModel>()
                };
            }
            catch
            {
                return new PageHourlyTrafficModel();
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
    }
}