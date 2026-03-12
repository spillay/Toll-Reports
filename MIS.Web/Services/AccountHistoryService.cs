using MIS.Web.Models.AccountHistory;
using Newtonsoft.Json;

namespace MIS.Web.Services
{
    public class AccountHistoryService : IAccountHistoryService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<AccountHistoryService> _logger;

        public AccountHistoryService(HttpClient httpClient, IConfiguration config, ILogger<AccountHistoryService> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
        }

        // ✅ FETCH ACCOUNT LIST (Dropdown)
        public async Task<List<string>> GetAccountsAsync()
        {
            string baseUrl = _config["BaseApiUrl:Link"];
            string endpoint = _config["ApiSettings:AccountListEndpoint"];
            string url = $"{baseUrl}{endpoint}";

            try
            {
                _logger.LogInformation("📌 Fetching account numbers → {Url}", url);

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("⚠ Account list fetch failed: HTTP {Code}", response.StatusCode);
                    return new();
                }

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<string>>(json) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error fetching accounts");
                return new();
            }
        }

        // ✅ FETCH ACCOUNT HISTORY

        public async Task<AccountHistoryInputModel> GetAccountHistoryAsync(
    string? accountNumber,
    DateTime? startDate,
    DateTime? endDate,
    bool? operational)
        {
            // If user did nothing yet, return empty model
            if (startDate == null && endDate == null && string.IsNullOrWhiteSpace(accountNumber))
                return new AccountHistoryInputModel { PageData = new PageAccountHistoryModel() };

            var baseUrl = (_config["BaseApiUrl:Link"] ?? "").TrimEnd('/');
            var endpoint = (_config["ApiSettings:AccountHistoryEndpoint"] ?? "").TrimStart('/');

            // Build query params
            var query = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(accountNumber))
                query["accountNumber"] = accountNumber.Trim();

            // ✅ keep time part if provided (better for datetime-local)
            if (startDate.HasValue)
                query["startDate"] = startDate.Value.ToString("yyyy-MM-ddTHH:mm:ss");

            if (endDate.HasValue)
                query["endDate"] = endDate.Value.ToString("yyyy-MM-ddTHH:mm:ss");

            if (operational.HasValue)
                query["operational"] = operational.Value ? "true" : "false";

            var queryString = string.Join("&", query.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));
            var url = $"{baseUrl}/{endpoint}?{queryString}";

            try
            {
                _logger.LogInformation("📘 Requesting Account History → {Url}", url);

                using var response = await _httpClient.GetAsync(url);

                // ✅ read once
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("❌ Account history API failed. Status={Status} Url={Url} Body={Body}",
                        (int)response.StatusCode, url, json);

                    return new AccountHistoryInputModel { PageData = new PageAccountHistoryModel() };
                }

                var payload = JsonConvert.DeserializeObject<AccountHistoryApiResponse>(json);

                if (payload == null)
                {
                    _logger.LogWarning("⚠ Invalid JSON payload for account {Acc}. Raw={Raw}", accountNumber, json);
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
                _logger.LogError(ex, "❌ Error fetching account history for {Acc}", accountNumber);
                return new AccountHistoryInputModel { PageData = new PageAccountHistoryModel() };
            }
        }
        public async Task<List<AccountSearchItem>> SearchAccountsAsync(string q, int take = 20)
        {
            q = (q ?? "").Trim();
            if (q.Length < 2) return new();

            take = Math.Clamp(take, 1, 50);

            string baseUrl = _config["BaseApiUrl:Link"];
            string endpoint = _config["ApiSettings:AccountHistorySearchEndpoint"]; // "api/AccountHistory/search-accounts"
            string url = $"{baseUrl}{endpoint}?q={Uri.EscapeDataString(q)}&take={take}";

            try
            {
                _logger.LogInformation("🔎 Searching accounts → {Url}", url);

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return new();

                var json = await response.Content.ReadAsStringAsync();

                // Deserialize into anonymous shape using Newtonsoft (safe)
                var raw = JsonConvert.DeserializeObject<List<dynamic>>(json) ?? new();

                // Map into a strong shape for your UI
                var results = raw.Select(x => new AccountSearchItem
                {
                    AccountNumber = (string?)x.accountNumber,
                    Description = (string?)x.description,
                    Balance = x.balance != null ? (decimal)x.balance : 0m
                }).ToList();

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error searching accounts");
                return new();
            }
        }

        // ✅ LOCAL API RESPONSE MODELS 
        private class AccountHistoryApiResponse
        {
            [JsonProperty("accountHeader")]
            public AccountHeader? AccountHeader { get; set; }

            [JsonProperty("historyRecords")]
            public List<AccountHistoryModel>? HistoryRecords { get; set; }

            // If your API returns these, keep them (otherwise remove)
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