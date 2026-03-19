using System.Text.Json;
using MIS.Web.Models.EndOfDay;
using MIS.Web.Services.Interfaces;

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

        public async Task<EndOfDayReportViewModel?> GetEndOfDayAsync(
            DateTime startDate,
            DateTime endDate,
            int? shiftId = null)
        {
            try
            {
                string baseUrl = _config["BaseApiUrl:Link"]?.TrimEnd('/')
                    ?? throw new Exception("BaseApiUrl:Link is missing in appsettings.json");

                string endpoint = _config["ApiSettings:EndOfDayEndpoint"]?.TrimStart('/')
                    ?? throw new Exception("ApiSettings:EndOfDayEndpoint is missing in appsettings.json");

                var queryParts = new List<string>
                {
                    $"startDate={Uri.EscapeDataString(startDate.ToString("yyyy-MM-ddTHH:mm:ss"))}",
                    $"endDate={Uri.EscapeDataString(endDate.ToString("yyyy-MM-ddTHH:mm:ss"))}"
                };

                if (shiftId.HasValue)
                {
                    queryParts.Add($"shiftId={shiftId.Value}");
                }

                string query = string.Join("&", queryParts);
                string url = $"{baseUrl}/{endpoint}?{query}";

                _logger.LogInformation(
                    "Fetching End Of Day report from: {Url}. StartDate: {StartDate:yyyy-MM-dd HH:mm:ss}, EndDate: {EndDate:yyyy-MM-dd HH:mm:ss}, ShiftId: {ShiftId}",
                    url,
                    startDate,
                    endDate,
                    shiftId);

                using var response = await _http.GetAsync(url);
                var responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("End Of Day raw response body: {ResponseBody}", responseBody);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError(
                        "End Of Day API request failed. StatusCode: {StatusCode}, Url: {Url}, Response: {Response}",
                        (int)response.StatusCode,
                        url,
                        responseBody);

                    return null;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<EndOfDayReportViewModel>(responseBody, options);

                if (result == null)
                {
                    _logger.LogWarning(
                        "End Of Day API returned data but deserialization resulted in null. Response: {Response}",
                        responseBody);
                    return null;
                }

                _logger.LogInformation(
                    "End Of Day web model loaded successfully. Rows: {Rows}, ShiftName: {ShiftName}, TotalTheoreticalIncome: {TotalTheoreticalIncome}",
                    result.TheoreticalIncome?.Count ?? 0,
                    result.ShiftName,
                    result.Totals?.TotalTheoreticalIncome ?? 0);

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