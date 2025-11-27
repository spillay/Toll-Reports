using MIS.Web.Models.EndOfDay;
using MIS.Web.Services.Interfaces;
using System.Net.Http.Json;

namespace MIS.Web.Services
{
    public class EndOfDayReportService : IEndOfDayReportService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly string _baseUrl;
        private readonly string _endpoint;

        public EndOfDayReportService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;

            _baseUrl = _config["BaseApiUrl:Link"]?.TrimEnd('/') + "/";
            _endpoint = _config["ApiSettings:EndOfDayEndpoint"] ?? "api/EndOfDayReport/Get";
        }

        public async Task<List<EndOfDayRowModel>> GetEndOfDayAsync(DateTime reportDate)
        {
            string formatted = reportDate.ToString("yyyy-MM-dd");
            string url = $"{_baseUrl}{_endpoint}?reportDate={formatted}";

            // fetch JSON safely
            var rows = await _http.GetFromJsonAsync<List<EndOfDayRowModel>>(url)
                       ?? new List<EndOfDayRowModel>();

            // normalize each row (trim all columns)
            foreach (var r in rows)
            {
                r.Col1 = r.Col1?.Trim() ?? "";
                r.Col2 = r.Col2?.Trim() ?? "";
                r.Col3 = r.Col3?.Trim() ?? "";
                r.Col4 = r.Col4?.Trim() ?? "";
                r.Col5 = r.Col5?.Trim() ?? "";
                r.Col6 = r.Col6?.Trim() ?? "";
                r.Col7 = r.Col7?.Trim() ?? "";
                r.Col8 = r.Col8?.Trim() ?? "";
            }

            // remove known headers
            string[] headersToRemove =
            {
                "LEKKI CONCESSION COMPANY LIMITED",
                "LEKKI-IKOYI LINK BRIDGE",
                "END OF DAY REPORT"
            };

            rows = rows
                .Where(r => !headersToRemove.Any(h =>
                        r.Col1.Equals(h, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            return rows;
        }
    }
}
