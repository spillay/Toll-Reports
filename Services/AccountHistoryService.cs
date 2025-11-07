using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MIS.Web.Models.AccountHistory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

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

        public async Task<List<string>> GetAccountsAsync()
        {
            string baseUrl = _config["BaseApiUrl:Link"];
            string endpoint = _config["ApiSettings:AccountListEndpoint"];
            string fullUrl = $"{baseUrl}{endpoint}";

            try
            {
                _logger.LogInformation("📡 Fetching account numbers from {Url}", fullUrl);

                using var response = await _httpClient.GetAsync(fullUrl);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("⚠️ Failed to fetch account list. HTTP {StatusCode}", response.StatusCode);
                    return new List<string>();
                }

                // The backend returns an array of strings: ["0100001", "0100002", ...]
                var json = await response.Content.ReadAsStringAsync();
                var accounts = JsonConvert.DeserializeObject<List<string>>(json);

                // Defensive check — avoid returning null
                return accounts ?? new List<string>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔥 Error fetching account numbers");
                return new List<string>();
            }
        }

        public async Task<AccountHistoryInputModel> GetAccountHistoryAsync(string accountNumber)
        {
            // Return empty model if no account number is passed
            if (string.IsNullOrWhiteSpace(accountNumber))
                return new AccountHistoryInputModel();

            string baseUrl = _config["BaseApiUrl:Link"];
            string endpoint = _config["ApiSettings:AccountHistoryEndpoint"];
            string fullUrl = $"{baseUrl}{endpoint}?accountNumber={Uri.EscapeDataString(accountNumber)}";

            try
            {
                _logger.LogInformation("📡 Fetching account history for account {AccountNumber}", accountNumber);

                using var response = await _httpClient.GetAsync(fullUrl);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("⚠️ Failed to load account history for {AccountNumber}. HTTP {StatusCode}",
                        accountNumber, response.StatusCode);
                    return new AccountHistoryInputModel();
                }

                // Deserialize the returned JSON structure
                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<AccountHistoryApiResponse>(json);

                if (payload == null || payload.AccountHeader == null)
                {
                    _logger.LogWarning("⚠️ No valid data received for account {AccountNumber}", accountNumber);
                    return new AccountHistoryInputModel();
                }

                // Map API response to view model
                return new AccountHistoryInputModel
                {
                    AccountNumber = payload.AccountHeader.AccountNumber,
                    AccountHolder = payload.AccountHeader.AccountHolder,
                    AccountStatus = payload.AccountHeader.AccountStatus,
                    AccountType = payload.AccountHeader.AccountType,
                    MobileNumber = payload.AccountHeader.MobileNumber,
                    Email = payload.AccountHeader.Email,
                    AccountBalance = payload.AccountHeader.AccountBalance,
                    PageData = new PageAccountHistoryModel
                    {
                        Items = payload.HistoryRecords ?? new List<AccountHistoryModel>()
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔥 Error loading account history for {AccountNumber}", accountNumber);
                return new AccountHistoryInputModel();
            }
        }
    }

  
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
