using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MIS.Web.Models.AccountUsageDetails;
using Newtonsoft.Json;

namespace MIS.Web.Services
{
    public class AccountUsageDetailsService : IAccountUsageDetailsService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<AccountUsageDetailsService> _logger;

        public AccountUsageDetailsService(
            HttpClient httpClient,
            IConfiguration config,
            ILogger<AccountUsageDetailsService> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
        }

        // =========================================================
        // GET DETAILS + SUMMARY (from GetDetails API)
        // =========================================================
        public async Task<PageAccountUsageDetailsModel> GetAccountUsageDetailsAsync(
            DateTime startDate, DateTime endDate)
        {
            var model = new PageAccountUsageDetailsModel();

            string baseUrl = _config["BaseApiUrl:Link"]?.TrimEnd('/');
            if (string.IsNullOrWhiteSpace(baseUrl))
                return model;

            string start = Uri.EscapeDataString(startDate.ToString("yyyy-MM-ddTHH:mm:ss"));
            string end = Uri.EscapeDataString(endDate.ToString("yyyy-MM-ddTHH:mm:ss"));

            string url = $"{baseUrl}/api/AccountUsageDetails/GetDetails?startDate={start}&endDate={end}";

            try
            {
                _logger.LogInformation("🌐 Calling API: {Url}", url);

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("⚠ API returned status {StatusCode}", response.StatusCode);
                    return model;
                }

                var json = await response.Content.ReadAsStringAsync();

                // Deserialize the WRAPPER { summary: {}, details: [] }
                var wrapper = JsonConvert.DeserializeObject<AccountUsageDetailsApiResponse>(json);

                model.Summary = wrapper?.Summary ?? new AccountUsageSummaryModel();
                model.Items = wrapper?.Details ?? new List<AccountUsageDetailsModel>();

                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error fetching Account Usage Details");
                return model;
            }
        }

        // =========================================================
        // NOT USED ANYMORE – Summary comes from GetDetails API
        // =========================================================
        public Task<AccountUsageSummaryModel> GetSummaryAsync()
        {
            return Task.FromResult(new AccountUsageSummaryModel());
        }
    }

    public class AccountUsageDetailsApiResponse
    {
        public AccountUsageSummaryModel? Summary { get; set; }
        public List<AccountUsageDetailsModel>? Details { get; set; }
    }
}
