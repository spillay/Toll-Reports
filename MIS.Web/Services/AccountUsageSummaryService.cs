using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MIS.Web.Models.AccountUsageSummary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public async Task<PageAccountUsageSummaryModel> GetAccountUsageSummaryAsync(DateTime startDate, DateTime endDate)
        {
            string baseUrl = _config["BaseApiUrl:Link"]?.TrimEnd('/');
            string endpoint = $"{baseUrl}/api/AccountUsageSummary/GetSummary?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";

            var response = await _httpClient.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                return new PageAccountUsageSummaryModel
                {
                    Summary = new AccountUsageSummarySummaryModel(),
                    Items = new List<AccountUsageSummaryModel>()
                };
            }

            var json = await response.Content.ReadAsStringAsync();

            var parsed = JsonConvert.DeserializeObject<PageAccountUsageSummaryModel>(json);

            if (parsed == null)
            {
                parsed = new PageAccountUsageSummaryModel
                {
                    Summary = new AccountUsageSummarySummaryModel(),
                    Items = new List<AccountUsageSummaryModel>()
                };
            }

            return parsed;
        }

    }
}
