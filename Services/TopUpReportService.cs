using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MIS.Web.Models;
using MIS.Web.Models.TopUp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MIS.Web.Services
{
    public class TopUpReportService : ITopUpReportService
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TopUpReportService(
            HttpClient client,
            IConfiguration config,
            IHttpContextAccessor httpContextAccessor)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<TopUpInputModel> GetTopUpFilterOptionsAsync()
        {
            try
            {
                var endpoint = _config["ApiSettings:TopUpFilterOptionsEndpoint"] ?? "api/TopUp/filter-options";
                var url = BuildUrl(endpoint, new List<string>());

                using var request = CreateAuthorizedGetRequest(url);
                var response = await _client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return new TopUpInputModel();

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TopUpInputModel>(json) ?? new TopUpInputModel();
            }
            catch
            {
                return new TopUpInputModel();
            }
        }

        public async Task<PageTopUpModel> GetTopUpAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? shifts = null,
            List<string>? operatorIds = null,
            List<string>? lanes = null,
            List<string>? paymentMethods = null,
            string? accountNumber = null,
            int page = 1,
            int pageSize = 30)
        {
            try
            {
                var endpoint = _config["ApiSettings:TopUpEndpoint"] ?? "api/TopUp/details";

                var query = new List<string>
                {
                    $"startDate={Uri.EscapeDataString(startDate.ToString("yyyy-MM-ddTHH:mm:ss"))}",
                    $"endDate={Uri.EscapeDataString(endDate.ToString("yyyy-MM-ddTHH:mm:ss"))}",
                    $"page={page}",
                    $"pageSize={pageSize}"
                };

                AddListParams(query, "shifts", shifts);
                AddListParams(query, "operatorIds", operatorIds);
                AddListParams(query, "lanes", lanes);
                AddListParams(query, "paymentMethods", paymentMethods);

                if (!string.IsNullOrWhiteSpace(accountNumber))
                    query.Add($"accountNumber={Uri.EscapeDataString(accountNumber.Trim())}");

                var url = BuildUrl(endpoint, query);

                using var request = CreateAuthorizedGetRequest(url);
                var response = await _client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return new PageTopUpModel();

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PageTopUpModel>(json) ?? new PageTopUpModel();
            }
            catch
            {
                return new PageTopUpModel();
            }
        }

        public async Task<PageTopUpModel> GetTopUpFullAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? shifts = null,
            List<string>? operatorIds = null,
            List<string>? lanes = null,
            List<string>? paymentMethods = null,
            string? accountNumber = null)
        {
            try
            {
                var endpoint = _config["ApiSettings:TopUpEndpoint"] ?? "api/TopUp/details";

                var query = new List<string>
                {
                    $"startDate={Uri.EscapeDataString(startDate.ToString("yyyy-MM-ddTHH:mm:ss"))}",
                    $"endDate={Uri.EscapeDataString(endDate.ToString("yyyy-MM-ddTHH:mm:ss"))}",
                    "page=1",
                    "pageSize=1000000"
                };

                AddListParams(query, "shifts", shifts);
                AddListParams(query, "operatorIds", operatorIds);
                AddListParams(query, "lanes", lanes);
                AddListParams(query, "paymentMethods", paymentMethods);

                if (!string.IsNullOrWhiteSpace(accountNumber))
                    query.Add($"accountNumber={Uri.EscapeDataString(accountNumber.Trim())}");

                var url = BuildUrl(endpoint, query);

                using var request = CreateAuthorizedGetRequest(url);
                var response = await _client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return new PageTopUpModel();

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PageTopUpModel>(json) ?? new PageTopUpModel();
            }
            catch
            {
                return new PageTopUpModel();
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

        private static void AddListParams(List<string> query, string key, List<string>? values)
        {
            if (values == null)
                return;

            foreach (var value in values.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()))
            {
                query.Add($"{key}={Uri.EscapeDataString(value)}");
            }
        }

        private string BuildUrl(string endpoint, List<string> queryParts)
        {
            var baseUrl = _config["BaseApiUrl:Link"]?.TrimEnd('/') ?? string.Empty;
            endpoint = (endpoint ?? string.Empty).TrimStart('/');

            var url = $"{baseUrl}/{endpoint}";

            if (queryParts != null && queryParts.Count > 0)
                url += "?" + string.Join("&", queryParts);

            return url;
        }
    }
}