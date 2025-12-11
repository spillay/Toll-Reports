using MIS.Web.Models.EndOfDay;
using MIS.Web.Services.Interfaces;
using System.Net.Http.Json;

namespace MIS.Web.Services
{
    public class EndOfDayReportService : IEndOfDayReportService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly ILogger<EndOfDayReportService> _logger;

        public EndOfDayReportService(
            HttpClient http,
            IConfiguration config,
            ILogger<EndOfDayReportService> logger)
        {
            _http = http;
            _config = config;
            _logger = logger;
        }

        public async Task<EndOfDayReportViewModel?> GetEndOfDayAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                string baseUrl = _config["BaseApiUrl:Link"]?.TrimEnd('/')
                                 ?? throw new Exception("BaseApiUrl:Link is missing in appsettings.json");

                // string url = $"{baseUrl}/api/EndOfDayReport?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
                string url = $"{baseUrl}/api/EndOfDayReport?startDate={startDate:yyyy-MM-ddTHH:mm:ss}&endDate={endDate:yyyy-MM-ddTHH:mm:ss}";


                _logger.LogInformation("Fetching End Of Day report from: {URL}", url);

                var result = await _http.GetFromJsonAsync<EndOfDayReportViewModel>(url);

                if (result == null)
                {
                    _logger.LogWarning("End Of Day API returned NULL data.");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch End Of Day report.");
                return null;
            }
        }
    }
}
