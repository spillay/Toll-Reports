using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MIS.Web.Services
{
    public class ApiClientService : IApiClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ApiClientService> _logger;

        public ApiClientService(
            HttpClient httpClient,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ApiClientService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string BuildUrl(string endpointConfigKey, IEnumerable<string>? queryParts = null)
        {
            var baseUrl = _configuration["BaseApiUrl:Link"];
            var endpoint = _configuration[endpointConfigKey];

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("BaseApiUrl:Link is missing in appsettings.json.");

            if (string.IsNullOrWhiteSpace(endpoint))
                throw new InvalidOperationException($"{endpointConfigKey} is missing in appsettings.json.");

            var url = $"{baseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";

            var query = queryParts?
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            if (query?.Any() == true)
                url += "?" + string.Join("&", query);

            return url;
        }

        public async Task<T?> GetAsync<T>(string endpointConfigKey, IEnumerable<string>? queryParts = null)
        {
            var url = BuildUrl(endpointConfigKey, queryParts);

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            AddBearerToken(request);

            using var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "API GET failed for {EndpointKey}. Status={StatusCode}. Body={Body}",
                    endpointConfigKey,
                    response.StatusCode,
                    json);

                return default;
            }

            return JsonConvert.DeserializeObject<T>(json);
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
