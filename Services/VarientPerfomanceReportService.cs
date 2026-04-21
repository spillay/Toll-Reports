using MIS.Web.Models.VarientPerfomance;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace MIS.Web.Services
{
    public class VarientPerfomanceReportService : IVarientPerfomanceReportService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VarientPerfomanceReportService(
            HttpClient httpClient,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<PageVarientPerfomanceModel> GetVarientPerfomanceDetailsAsync(
            int pageNumber,
            int pageSize,
            DateTime startDate,
            DateTime endDate,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null)
        {
            try
            {
                var queryParts = new List<string>
                {
                    $"startDate={Uri.EscapeDataString(startDate.ToString("s"))}",
                    $"endDate={Uri.EscapeDataString(endDate.ToString("s"))}",
                    $"page={pageNumber}",
                    $"pageSize={pageSize}"
                };

                AddIfAny(queryParts, "operationalShift", operationalShift);
                AddIfAny(queryParts, "tollOperators", tollOperators);

                var url = BuildUrlFromConfig("ApiSettings:VarientPerformanceEndpoint", queryParts);

                using var request = CreateAuthorizedGetRequest(url);
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return new PageVarientPerfomanceModel();

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PageVarientPerfomanceModel>(json)
                       ?? new PageVarientPerfomanceModel();
            }
            catch
            {
                return new PageVarientPerfomanceModel();
            }
        }

        public async Task<List<string>> GetAllShiftsAsync()
        {
            try
            {
                var url = BuildUrlFromConfig("ApiSettings:VarientPerformanceShiftsEndpoint");

                using var request = CreateAuthorizedGetRequest(url);
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return new List<string>();

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();

                return result
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        public async Task<List<string>> GetAllTollOperatorsAsync()
        {
            try
            {
                var url = BuildUrlFromConfig("ApiSettings:VarientPerformanceOperatorsEndpoint");

                using var request = CreateAuthorizedGetRequest(url);
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return new List<string>();

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();

                return result
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
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

        private static void AddIfAny(List<string> queryParts, string key, List<string>? list)
        {
            if (list == null || !list.Any())
                return;

            foreach (var value in list.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                queryParts.Add($"{key}={Uri.EscapeDataString(value.Trim())}");
            }
        }

        private string BuildUrlFromConfig(string endpointKey, IEnumerable<string>? queryParts = null)
        {
            var baseUrl = _configuration["BaseApiUrl:Link"];
            var endpoint = _configuration[endpointKey];

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("BaseApiUrl:Link is missing in appsettings.json.");

            if (string.IsNullOrWhiteSpace(endpoint))
                throw new InvalidOperationException($"{endpointKey} is missing in appsettings.json.");

            var url = $"{baseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";

            if (queryParts != null && queryParts.Any())
                url += "?" + string.Join("&", queryParts);

            return url;
        }
    }
}