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

        // ============================================================
        // FETCH ACCOUNT LIST
        // ============================================================
        public async Task<List<string>> GetAccountsAsync()
        {
            string baseUrl = _config["BaseApiUrl:Link"];
            string endpoint = _config["ApiSettings:AccountListEndpoint"];
            string fullUrl = $"{baseUrl}{endpoint}";

            try
            {
                _logger.LogInformation("📌 Fetching account numbers → {Url}", fullUrl);

                var response = await _httpClient.GetAsync(fullUrl);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("⚠ Account list fetch failed: HTTP {Code}", response.StatusCode);
                    return new();
                }

                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<string>>(json) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error fetching accounts");
                return new();
            }
        }

        // ============================================================
        // FETCH ACCOUNT HISTORY (With logic for operational)
        // ============================================================
        public async Task<AccountHistoryInputModel> GetAccountHistoryAsync(
            string accountNumber,
            DateTime? startDate,
            DateTime? endDate,
            bool? operational)
        {
            // Return empty if no filters applied yet
            if (startDate == null && endDate == null && string.IsNullOrWhiteSpace(accountNumber))
            {
                return new AccountHistoryInputModel();
            }

            string baseUrl = _config["BaseApiUrl:Link"];
            string endpoint = _config["ApiSettings:AccountHistoryEndpoint"];

            // BUILD QUERY PARAMETERS
            var query = new Dictionary<string, string>();

            // Only include accountNumber if operational = true
            if (operational == true && !string.IsNullOrWhiteSpace(accountNumber))
                query["accountNumber"] = accountNumber;

            if (startDate.HasValue)
                query["startDate"] = startDate.Value.ToString("yyyy-MM-ddTHH:mm:ss");

            if (endDate.HasValue)
                query["endDate"] = endDate.Value.ToString("yyyy-MM-ddTHH:mm:ss");

            // Always pass operational state
            if (operational.HasValue)
                query["operational"] = operational.Value.ToString().ToLower();

            string queryString = string.Join("&",
                query.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));

            string fullUrl = $"{baseUrl}{endpoint}?{queryString}";

            try
            {
                _logger.LogInformation("📘 Requesting Account History → {Url}", fullUrl);

                var response = await _httpClient.GetAsync(fullUrl);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("⚠ Failed to fetch account history: HTTP {Code}", response.StatusCode);
                    return new AccountHistoryInputModel();
                }

                string json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<AccountHistoryApiResponse>(json);

                if (payload == null)
                {
                    _logger.LogWarning("⚠ Empty response payload for account {Acc}", accountNumber);
                    return new AccountHistoryInputModel();
                }

                return new AccountHistoryInputModel
                {
                    // HEADER
                    AccountNumber = payload.AccountHeader?.AccountNumber ?? accountNumber,
                    AccountHolder = payload.AccountHeader?.AccountHolder,
                    AccountStatus = payload.AccountHeader?.AccountStatus,
                    AccountType = payload.AccountHeader?.AccountType,
                    MobileNumber = payload.AccountHeader?.MobileNumber,
                    Email = payload.AccountHeader?.Email,
                    AccountBalance = (double)(payload.AccountHeader?.AccountBalance ?? 0),

                    // FILTERS
                    StartDate = startDate,
                    EndDate = endDate,
                    Operational = operational,

                    // DATA
                    PageData = new PageAccountHistoryModel
                    {
                        Items = payload.HistoryRecords ?? new(),
                        totalCount = payload.HistoryRecords?.Count ?? 0,
                        page = 1,
                        pageSize = payload.HistoryRecords?.Count ?? 50,
                        totalPages = 1
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error fetching account history for {Acc}", accountNumber);
                return new AccountHistoryInputModel();
            }
        }
    }

    // ============================================================
    // API RESPONSE MODELS
    // ============================================================
    public class AccountHistoryApiResponse
    {
        [JsonProperty("accountHeader")]
        public AccountHeader AccountHeader { get; set; }

        [JsonProperty("historyRecords")]
        public List<AccountHistoryModel> HistoryRecords { get; set; }
    }

    public class AccountHeader
    {
        [JsonProperty("accountNumber")]
        public string AccountNumber { get; set; }

        [JsonProperty("accountHolder")]
        public string AccountHolder { get; set; }

        [JsonProperty("accountStatus")]
        public string AccountStatus { get; set; }

        [JsonProperty("accountType")]
        public string AccountType { get; set; }

        [JsonProperty("mobileNumber")]
        public string MobileNumber { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("accountBalance")]
        public decimal AccountBalance { get; set; }
    }
}
