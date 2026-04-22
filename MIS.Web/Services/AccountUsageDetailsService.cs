using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MIS.Web.Models.AccountUsageDetails;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MIS.Web.Services
{
    public class AccountUsageDetailsService : IAccountUsageDetailsService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<AccountUsageDetailsService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountUsageDetailsService(
            HttpClient httpClient,
            IConfiguration config,
            ILogger<AccountUsageDetailsService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetBaseUrl()
        {
            return _config["BaseApiUrl:Link"]?.TrimEnd('/') ?? string.Empty;
        }

        public async Task<List<AccountSearchResultModel>> SearchAccountsAsync(string q, int take = 20)
        {
            var baseUrl = GetBaseUrl();
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                _logger.LogWarning("Base API URL is missing for AccountUsageDetails search.");
                return new List<AccountSearchResultModel>();
            }

            q = (q ?? string.Empty).Trim();
            if (q.Length < 3)
            {
                return new List<AccountSearchResultModel>();
            }

            take = Math.Clamp(take, 1, 50);

            var url = $"{baseUrl}/api/AccountUsageDetails/SearchAccounts" +
                      $"?q={Uri.EscapeDataString(q)}&take={take}";

            try
            {
                _logger.LogInformation("Calling AccountUsageDetails SearchAccounts API: {Url}", url);

                using var request = CreateAuthorizedGetRequest(url);
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
                using var response = await _httpClient.SendAsync(request, cts.Token);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "SearchAccounts API returned status code {StatusCode} for query {Query}",
                        response.StatusCode,
                        q);

                    return new List<AccountSearchResultModel>();
                }

                var json = await response.Content.ReadAsStringAsync(cts.Token);

                var results = JsonConvert.DeserializeObject<List<AccountSearchResultModel>>(json)
                              ?? new List<AccountSearchResultModel>();

                return results
                    .Where(x => !string.IsNullOrWhiteSpace(x.AccountNumber))
                    .Select(x => new AccountSearchResultModel
                    {
                        AccountNumber = x.AccountNumber?.Trim() ?? string.Empty,
                        Description = x.Description?.Trim() ?? string.Empty
                    })
                    .ToList();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogWarning(ex, "AccountUsageDetails search timed out for query {Query}", q);
                return new List<AccountSearchResultModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling SearchAccounts API for query {Query}", q);
                return new List<AccountSearchResultModel>();
            }
        }

        public async Task<PageAccountUsageDetailsModel> GetAccountUsageDetailsAsync(
            string accountNumber,
            DateTime startDate,
            DateTime endDate)
        {
            var model = new PageAccountUsageDetailsModel
            {
                Header = new AccountUsageDetailsHeaderModel(),
                Items = new List<AccountUsageDetailsRowModel>()
            };

            var baseUrl = GetBaseUrl();
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                _logger.LogWarning("Base API URL is missing for AccountUsageDetails details request.");
                return model;
            }

            accountNumber = (accountNumber ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                _logger.LogWarning("Account number is required for AccountUsageDetails details request.");
                return model;
            }

            var start = Uri.EscapeDataString(startDate.ToString("yyyy-MM-ddTHH:mm:ss"));
            var end = Uri.EscapeDataString(endDate.ToString("yyyy-MM-ddTHH:mm:ss"));
            var acc = Uri.EscapeDataString(accountNumber);

            var url = $"{baseUrl}/api/AccountUsageDetails/GetDetails" +
                      $"?accountNumber={acc}&startDate={start}&endDate={end}";

            try
            {
                _logger.LogInformation("Calling AccountUsageDetails GetDetails API: {Url}", url);

                using var request = CreateAuthorizedGetRequest(url);
                using var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "GetDetails API returned status code {StatusCode} for account {AccountNumber}",
                        response.StatusCode,
                        accountNumber);

                    return model;
                }

                var json = await response.Content.ReadAsStringAsync();

                var apiResponse = JsonConvert.DeserializeObject<AccountUsageDetailsResponse>(json)
                                  ?? new AccountUsageDetailsResponse();

                model.Header = apiResponse.Header ?? new AccountUsageDetailsHeaderModel();
                model.Items = apiResponse.Details ?? new List<AccountUsageDetailsRowModel>();

                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching Account Usage Details for account {AccountNumber} between {StartDate} and {EndDate}",
                    accountNumber,
                    startDate,
                    endDate);

                return model;
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