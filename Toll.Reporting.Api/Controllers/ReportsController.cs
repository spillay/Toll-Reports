using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;

namespace Toll.Reporting.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ILogger<ReportsController> _logger;
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public ReportsController(IConfiguration config, ILogger<ReportsController> logger)
        {
            _logger = logger;
            _config = config;

            // Read settings directly from appsettings.json
            var reportServerUrl = _config["SSRS:ReportServerUrl"]; // <-- expects ReportServer URL, not Reports
            var ssrsUsername = _config["SSRS:SsrsUsername"];
            var ssrsPassword = _config["SSRS:SsrsPassword"];
            var ssrsDomain = _config["SSRS:SsrsDomain"];
            var timeoutSeconds = int.TryParse(_config["SSRS:TimeoutSeconds"], out var t) ? t : 60;

            // Setup HTTP client handler (with credentials if provided)
            var handler = new HttpClientHandler();
            if (!string.IsNullOrEmpty(ssrsUsername))
            {
                handler.Credentials = new NetworkCredential(
                    ssrsUsername,
                    ssrsPassword,
                    ssrsDomain
                );
            }

            _httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(timeoutSeconds)
            };
        }

        /// <summary>
        /// Returns HTML content for the specified report and parameters.
        /// </summary>
        [HttpGet("embed")]
        public async Task<IActionResult> GetReportEmbed(
            [FromQuery] string reportName,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] List<string>? operationalShift = null,
            [FromQuery] List<string>? tollOperator = null,
            [FromQuery] List<string>? laneName = null,
            [FromQuery] List<string>? paymentMethod = null)
        {
            if (string.IsNullOrEmpty(reportName))
                return BadRequest("reportName is required.");

            // Read values from config
            var reportServerUrl = _config["SSRS:ReportServerUrl"]; // <-- make sure this points to http://SERVER/ReportServer
            var reportFolder = _config["SSRS:ReportFolder"];

            // Build SSRS URL (use /Pages/ReportViewer.aspx for ReportServer rendering)
            var reportPath = $"{reportFolder}/{reportName}";
            var sb = new StringBuilder();

            // Changed here: explicitly target ReportServer viewer
            sb.Append($"{reportServerUrl}/Pages/ReportViewer.aspx?{Uri.EscapeDataString(reportPath)}"); // <-- modified

            // Add required parameters
            sb.Append($"&StartDate={Uri.EscapeDataString(startDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "")}");
            sb.Append($"&EndDate={Uri.EscapeDataString(endDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "")}");

            // Append optional parameters
            AppendMultiValueParam(sb, "OperationalShift", operationalShift);
            AppendMultiValueParam(sb, "TollOperator", tollOperator);
            AppendMultiValueParam(sb, "LaneName", laneName);
            AppendMultiValueParam(sb, "PaymentMethod", paymentMethod);

            // Render as HTML
            sb.Append("&rs:Command=Render&rs:Format=HTML4.0");

            var ssrsUrl = sb.ToString();
            _logger.LogInformation("SSRS Embed URL: {Url}", ssrsUrl);

            try
            {
                using var response = await _httpClient.GetAsync(ssrsUrl);
                response.EnsureSuccessStatusCode();

                var html = await response.Content.ReadAsStringAsync();
                return Content(html, "text/html");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching report from SSRS");
                return StatusCode(502, "Error retrieving report from report server.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching report.");
                return StatusCode(500, "Unexpected error retrieving the report.");
            }
        }

        /// <summary>
        /// Returns PDF content for the specified report and parameters.
        /// </summary>
        [HttpGet("pdf")]
        public async Task<IActionResult> GetReportPdf(
            [FromQuery] string reportName,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] List<string>? operationalShift = null,
            [FromQuery] List<string>? tollOperator = null,
            [FromQuery] List<string>? laneName = null,
            [FromQuery] List<string>? paymentMethod = null)
        {
            if (string.IsNullOrEmpty(reportName))
                return BadRequest("reportName is required.");

            var reportServerUrl = _config["SSRS:ReportServerUrl"];
            var reportFolder = _config["SSRS:ReportFolder"];

            var reportPath = $"{reportFolder}/{reportName}";
            var sb = new StringBuilder();

            // Changed here: explicitly target ReportServer
            sb.Append($"{reportServerUrl}?{Uri.EscapeDataString(reportPath)}"); // <-- modified

            sb.Append($"&StartDate={Uri.EscapeDataString(startDate?.ToString("yyyy-MM-dd") ?? "")}");
            sb.Append($"&EndDate={Uri.EscapeDataString(endDate?.ToString("yyyy-MM-dd") ?? "")}");

            AppendMultiValueParam(sb, "OperationalShift", operationalShift);
            AppendMultiValueParam(sb, "TollOperator", tollOperator);
            AppendMultiValueParam(sb, "LaneName", laneName);
            AppendMultiValueParam(sb, "PaymentMethod", paymentMethod);

            // Force PDF rendering
            sb.Append("&rs:Command=Render&rs:Format=PDF"); // <-- PDF renderer

            var ssrsUrl = sb.ToString();
            _logger.LogInformation("SSRS PDF URL: {Url}", ssrsUrl);

            try
            {
                using var response = await _httpClient.GetAsync(ssrsUrl);
                response.EnsureSuccessStatusCode();

                var bytes = await response.Content.ReadAsByteArrayAsync();
                return File(bytes, "application/pdf", $"{reportName}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving PDF from SSRS");
                return StatusCode(500, "Error generating PDF.");
            }
        }

        /// <summary>
        /// Helper method to append multiple values for a query parameter.
        /// </summary>
        private void AppendMultiValueParam(StringBuilder sb, string key, List<string>? values)
        {
            if (values == null) return;

            foreach (var value in values)
            {
                sb.Append($"&{key}={Uri.EscapeDataString(value)}");
            }
        }
    }
}