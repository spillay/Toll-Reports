using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MIS.Web.Models.AccountUsageSummary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MIS.Web.Services
{
    public class AccountUsageSummaryService : IAccountUsageSummaryService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<AccountUsageSummaryService> _logger;

        public AccountUsageSummaryService(
            HttpClient httpClient,
            IConfiguration config,
            ILogger<AccountUsageSummaryService> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
        }

        private string GetBaseUrl()
        {
            return _config["BaseApiUrl:Link"]?.TrimEnd('/') ?? string.Empty;
        }

        public async Task<PageAccountUsageSummaryModel> GetAccountUsageSummaryAsync(
            DateTime startDate,
            DateTime endDate,
            string? accountNumber = null,
            int page = 1,
            int pageSize = 20)
        {
            var model = new PageAccountUsageSummaryModel
            {
                Summary = new AccountUsageSummarySummaryModel(),
                Items = new List<AccountUsageSummaryModel>(),
                Filters = new AccountUsageSummaryInputModel
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    AccountNumber = accountNumber,
                    Page = page,
                    PageSize = pageSize
                },
                page = page,
                pageSize = pageSize,
                totalCount = 0,
                totalPages = 0
            };

            var baseUrl = GetBaseUrl();
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                _logger.LogWarning("Base API URL is missing for AccountUsageSummary request.");
                return model;
            }

            accountNumber = (accountNumber ?? string.Empty).Trim();

            var start = Uri.EscapeDataString(startDate.ToString("yyyy-MM-ddTHH:mm:ss"));
            var end = Uri.EscapeDataString(endDate.ToString("yyyy-MM-ddTHH:mm:ss"));

            var url = $"{baseUrl}/api/AccountUsageSummary/GetSummary" +
                      $"?startDate={start}" +
                      $"&endDate={end}" +
                      $"&page={page}" +
                      $"&pageSize={pageSize}";

            if (!string.IsNullOrWhiteSpace(accountNumber))
            {
                url += $"&accountNumber={Uri.EscapeDataString(accountNumber)}";
            }

            try
            {
                _logger.LogInformation("Calling AccountUsageSummary API: {Url}", url);

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();

                    _logger.LogWarning(
                        "AccountUsageSummary API returned status code {StatusCode}. Url: {Url}. Response: {Response}",
                        response.StatusCode,
                        url,
                        errorBody);

                    return model;
                }

                var json = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(json))
                {
                    _logger.LogWarning("AccountUsageSummary API returned empty response. Url: {Url}", url);
                    return model;
                }

                var parsed = JsonConvert.DeserializeObject<AccountUsageSummaryApiResponseModel>(json);
                if (parsed == null)
                {
                    _logger.LogWarning("Failed to deserialize AccountUsageSummary API response. Url: {Url}", url);
                    return model;
                }

                model.Summary = parsed.Summary ?? new AccountUsageSummarySummaryModel();
                model.Items = parsed.Data?.Items ?? new List<AccountUsageSummaryModel>();

                model.page = parsed.Data?.Page ?? page;
                model.pageSize = parsed.Data?.PageSize ?? pageSize;
                model.totalCount = parsed.Data?.TotalCount ?? 0;
                model.totalPages = parsed.Data?.TotalPages ?? 0;

                model.Filters = new AccountUsageSummaryInputModel
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    AccountNumber = accountNumber,
                    Page = model.page,
                    PageSize = model.pageSize
                };

                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching Account Usage Summary for account {AccountNumber} between {StartDate} and {EndDate}",
                    accountNumber,
                    startDate,
                    endDate);

                return model;
            }
        }
    }
}