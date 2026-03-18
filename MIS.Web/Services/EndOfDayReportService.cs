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

        public async Task<EndOfDayReportViewModel?> GetEndOfDayAsync(DateTime startDate, DateTime endDate, int? shiftId = null)
        {
            try
            {
                string baseUrl = _config["BaseApiUrl:Link"]?.TrimEnd('/')
                    ?? throw new Exception("BaseApiUrl:Link is missing in appsettings.json");

                string endpoint = _config["ApiSettings:EndOfDayEndpoint"]?.TrimStart('/')
                    ?? throw new Exception("ApiSettings:EndOfDayEndpoint is missing in appsettings.json");

                var query = $"startDate={Uri.EscapeDataString(startDate.ToString("dd/MM/yyyy"))}" +
                            $"&endDate={Uri.EscapeDataString(endDate.ToString("dd/MM/yyyy"))}";

                if (shiftId.HasValue)
                {
                    query += $"&shiftId={shiftId.Value}";
                }

                string url = $"{baseUrl}/{endpoint}?{query}";

                _logger.LogInformation(
                    "Fetching End Of Day report from: {URL}. StartDate: {StartDate}, EndDate: {EndDate}, ShiftId: {ShiftId}",
                    url,
                    startDate,
                    endDate,
                    shiftId);

                var result = await _http.GetFromJsonAsync<EndOfDayReportViewModel>(url);

                if (result == null)
                {
                    _logger.LogWarning(
                        "End Of Day API returned null data. StartDate: {StartDate}, EndDate: {EndDate}, ShiftId: {ShiftId}",
                        startDate,
                        endDate,
                        shiftId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to fetch End Of Day report. StartDate: {StartDate}, EndDate: {EndDate}, ShiftId: {ShiftId}",
                    startDate,
                    endDate,
                    shiftId);

                return null;
            }
        }
    }
}