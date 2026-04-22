using MIS.Web.Models.AccountHistory;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MIS.Web.Services
{
    public class AccountHistoryService : IAccountHistoryService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<AccountHistoryService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountHistoryService(
            HttpClient httpClient,
            IConfiguration config,
            ILogger<AccountHistoryService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<string>> GetAccountsAsync()
        {
            try
            {
                var url = BuildUrl("ApiSettings:AccountListEndpoint");
                _logger.LogInformation("Fetching account numbers → {Url}", url);

                using var request = CreateAuthorizedGetRequest(url);
                using var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Account list fetch failed: HTTP {Code}", response.StatusCode);
                    return new();
                }

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<string>>(json) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching accounts");
                return new();
            }
        }

        public async Task<AccountHistoryInputModel> GetAccountHistoryAsync(
            string? accountNumber,
            DateTime? startDate,
            DateTime? endDate,
            bool? operational)
        {
            if (startDate == null && endDate == null && string.IsNullOrWhiteSpace(accountNumber))
                return new AccountHistoryInputModel { PageData = new PageAccountHistoryModel() };

            var query = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(accountNumber))
                query["accountNumber"] = accountNumber.Trim();

            if (startDate.HasValue)
                query["startDate"] = startDate.Value.ToString("yyyy-MM-ddTHH:mm:ss");

            if (endDate.HasValue)
                query["endDate"] = endDate.Value.ToString("yyyy-MM-ddTHH:mm:ss");

            if (operational.HasValue)
                query["operational"] = operational.Value ? "true" : "false";

            var url = BuildUrl("ApiSettings:AccountHistoryEndpoint", query);

            try
            {
                _logger.LogInformation("Requesting Account History → {Url}", url);

                using var request = CreateAuthorizedGetRequest(url);
                using var response = await _httpClient.SendAsync(request);

                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Account history API failed. Status={Status} Url={Url} Body={Body}",
                        (int)response.StatusCode, url, json);

                    return new AccountHistoryInputModel { PageData = new PageAccountHistoryModel() };
                }

                var payload = JsonConvert.DeserializeObject<AccountHistoryApiResponse>(json);

                if (payload == null)
                {
                    _logger.LogWarning("Invalid JSON payload for account {Acc}. Raw={Raw}", accountNumber, json);
                    return new AccountHistoryInputModel { PageData = new PageAccountHistoryModel() };
                }

                var records = payload.HistoryRecords ?? new List<AccountHistoryModel>();

                var pageData = new PageAccountHistoryModel
                {
                    Items = records,
                    totalCount = records.Count,
                    page = 1,
                    pageSize = Math.Max(records.Count, 50),
                    totalPages = 1
                };

                return new AccountHistoryInputModel
                {
                    AccountNumber = payload.AccountHeader?.AccountNumber ?? accountNumber,
                    AccountHolder = payload.AccountHeader?.AccountHolder,
                    AccountStatus = payload.AccountHeader?.AccountStatus,
                    AccountType = payload.AccountHeader?.AccountType,
                    MobileNumber = payload.AccountHeader?.MobileNumber,
                    Email = payload.AccountHeader?.Email,
                    AccountBalance = (double)(payload.AccountHeader?.AccountBalance ?? 0),

                    StartDate = startDate,
                    EndDate = endDate,
                    Operational = operational,

                    PageData = pageData,
                    FullRecords = new()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching account history for {Acc}", accountNumber);
                return new AccountHistoryInputModel { PageData = new PageAccountHistoryModel() };
            }
        }

        public async Task<List<AccountSearchItem>> SearchAccountsAsync(string q, int take = 20)
        {
            q = (q ?? "").Trim();

            if (q.Length < 3)
                return new();

            take = Math.Clamp(take, 1, 50);

            var query = new Dictionary<string, string>
            {
                ["q"] = q,
                ["take"] = take.ToString()
            };

            var url = BuildUrl("ApiSettings:AccountHistorySearchEndpoint", query);

            try
            {
                _logger.LogInformation("Searching accounts → {Url}", url);

                using var request = CreateAuthorizedGetRequest(url);

                // Shorter timeout just for live search
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));

                using var response = await _httpClient.SendAsync(request, cts.Token);

                if (!response.IsSuccessStatusCode)
                    return new();

                var json = await response.Content.ReadAsStringAsync(cts.Token);
                var raw = JsonConvert.DeserializeObject<List<dynamic>>(json) ?? new();

                return raw.Select(x => new AccountSearchItem
                {
                    AccountNumber = (string?)x.accountNumber,
                    Description = (string?)x.description,
                    Balance = x.balance != null ? (decimal)x.balance : 0m
                }).ToList();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogWarning(ex, "Account search timed out for query {Query}", q);
                return new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching accounts");
                return new();
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

        private string BuildUrl(string endpointConfigKey, IDictionary<string, string>? query = null)
        {
            var baseUrl = (_config["BaseApiUrl:Link"] ?? "").TrimEnd('/');
            var endpoint = (_config[endpointConfigKey] ?? "").TrimStart('/');

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("BaseApiUrl:Link is missing in configuration.");

            if (string.IsNullOrWhiteSpace(endpoint))
                throw new InvalidOperationException($"{endpointConfigKey} is missing in configuration.");

            var url = $"{baseUrl}/{endpoint}";

            if (query != null && query.Count > 0)
            {
                var queryString = string.Join("&",
                    query.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));

                url += "?" + queryString;
            }

            return url;
        }

        private class AccountHistoryApiResponse
        {
            [JsonProperty("accountHeader")]
            public AccountHeader? AccountHeader { get; set; }

            [JsonProperty("historyRecords")]
            public List<AccountHistoryModel>? HistoryRecords { get; set; }

            [JsonProperty("totalTopUps")]
            public decimal TotalTopUps { get; set; }

            [JsonProperty("totalTransactions")]
            public decimal TotalTransactions { get; set; }

            [JsonProperty("netMovement")]
            public decimal NetMovement { get; set; }
        }

        private class AccountHeader
        {
            [JsonProperty("accountNumber")]
            public string? AccountNumber { get; set; }

            [JsonProperty("accountHolder")]
            public string? AccountHolder { get; set; }

            [JsonProperty("accountStatus")]
            public string? AccountStatus { get; set; }

            [JsonProperty("accountType")]
            public string? AccountType { get; set; }

            [JsonProperty("mobileNumber")]
            public string? MobileNumber { get; set; }

            [JsonProperty("email")]
            public string? Email { get; set; }

            [JsonProperty("accountBalance")]
            public decimal AccountBalance { get; set; }
        }
    }
}