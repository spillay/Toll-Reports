using MIS.Web.Models.Comprehensive;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MIS.Web.Services
{
    public class ComprehensiveReportService : IComprehensiveReportService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ComprehensiveReportService(
            HttpClient httpClient,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<ComprehensiveOptionsResponse> GetComprehensiveOptionsAsync()
        {
            try
            {
                var baseUrl = _configuration["BaseApiUrl:Link"];
                var endpoint = _configuration["ApiSettings:ComprehensiveOptionsEndpoint"];
                var url = CombineUrl(baseUrl, endpoint);

                using var request = CreateAuthorizedGetRequest(url);
                using var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return new ComprehensiveOptionsResponse();

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ComprehensiveOptionsResponse>(json)
                       ?? new ComprehensiveOptionsResponse();
            }
            catch
            {
                return new ComprehensiveOptionsResponse();
            }
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
            try
            {
                var baseUrl = _configuration["BaseApiUrl:Link"];
                var endpoint = _configuration["ApiSettings:ComprehensiveReportEndpoint"];
                var url = CombineUrl(baseUrl, endpoint);

                var query = new List<string>
                {
                    $"startDate={Uri.EscapeDataString(startDate.ToString("yyyy-MM-dd"))}",
                    $"endDate={Uri.EscapeDataString(endDate.ToString("yyyy-MM-dd"))}"
                };

                AddList(query, "shiftIds", shiftIds);
                AddList(query, "operatorIds", operatorIds);
                AddList(query, "laneIds", laneIds);
                AddList(query, "discountTypeIds", discountTypeIds);
                AddList(query, "tollClassIds", tollClassIds);
                AddList(query, "paymentMethodIds", paymentMethodIds);

                var fullUrl = $"{url}?{string.Join("&", query)}";

                using var request = CreateAuthorizedGetRequest(fullUrl);
                using var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return new List<ComprehensiveModel>();

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ComprehensiveModel>>(json)
                       ?? new List<ComprehensiveModel>();
            }
            catch
            {
                return new List<ComprehensiveModel>();
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

        private static string CombineUrl(string baseUrl, string endpoint)
        {
            baseUrl = (baseUrl ?? "").TrimEnd('/');
            endpoint = (endpoint ?? "").TrimStart('/');
            return $"{baseUrl}/{endpoint}";
        }

        private static void AddList<T>(List<string> query, string key, IEnumerable<T>? list)
        {
            if (list == null) return;

            foreach (var value in list)
            {
                query.Add($"{key}={Uri.EscapeDataString(Convert.ToString(value) ?? string.Empty)}");
            }
        }
    }
}