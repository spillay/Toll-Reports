using MIS.Web.Models.Discrepancy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MIS.Web.Services
{
    public class DiscrepancyReportService : IDiscrepancyReportService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DiscrepancyReportService> _logger;

        public DiscrepancyReportService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<DiscrepancyReportService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<PageDiscrepancyModel> GetDiscrepancyReportAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null,
            List<string>? laneNames = null,
            List<string>? paymentMethods = null,
            List<string>? takenAction = null,
            int page = 1,
            int pageSize = 50)
        {
            try
            {
                // -------------------------
                // BUILD QUERY STRING
                // -------------------------
                var q = new List<string>
                {
                    $"startDate={Uri.EscapeDataString(startDate.ToString("s"))}",
                    $"endDate={Uri.EscapeDataString(endDate.ToString("s"))}",
                    $"page={page}",
                    $"pageSize={pageSize}"
                };

                void AddList(string key, List<string>? list)
                {
                    if (list?.Any() == true)
                    {
                        foreach (var v in list)
                            q.Add($"{key}={Uri.EscapeDataString(v)}");
                    }
                }

                AddList("operationalShift", operationalShift);
                AddList("tollOperators", tollOperators);
                AddList("laneNames", laneNames);
                AddList("paymentMethods", paymentMethods);
                AddList("takenAction", takenAction);

                // -------------------------
                // BUILD URL
                // -------------------------
                string baseUrl = _configuration["BaseApiUrl:Link"];
                string endpoint = _configuration["ApiSettings:DiscrepancyReportEndpoint"];

                string url = $"{baseUrl}{endpoint}?{string.Join("&", q)}";

                _logger.LogInformation($"📡 Fetching Discrepancy Report from: {url}");

                // -------------------------
                // SEND REQUEST
                // -------------------------
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"❌ Failed to fetch discrepancy report. Status: {response.StatusCode}");
                    return EmptyResult(startDate, endDate, page, pageSize);
                }

                var body = await response.Content.ReadAsStringAsync();

                // -------------------------
                // JSON DESERIALIZATION FIX
                // -------------------------
                var pagedResult = JsonConvert.DeserializeObject<PageDiscrepancyModel>(
                    body,
                    new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    });

                if (pagedResult == null)
                {
                    _logger.LogWarning("⚠️ API returned NULL or invalid JSON for Discrepancy Report");
                    return EmptyResult(startDate, endDate, page, pageSize);
                }

                // -------------------------
                // FILL FILTER INFO
                // -------------------------
                pagedResult.Filters = new DiscrepancyInputModel
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Shift = operationalShift?.FirstOrDefault(),
                    toll_Operator_ID = tollOperators?.FirstOrDefault(),
                    lane_Nr = laneNames?.FirstOrDefault(),
                    PaymentMethod = paymentMethods?.FirstOrDefault(),
                    TakenAction = takenAction?.FirstOrDefault(),
                    Page = page,
                    PageSize = pageSize
                };

                return pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 ERROR: Discrepancy service crashed.");
                return EmptyResult(startDate, endDate, page, pageSize);
            }
        }

        public async Task<PageDiscrepancyModel> GetFullExportAsync(DiscrepancyInputModel f)
        {
            // Build URL for full export (NO pagination!)
            var q = new List<string>
    {
        $"startDate={Uri.EscapeDataString(f.StartDate.ToString("s"))}",
        $"endDate={Uri.EscapeDataString(f.EndDate.ToString("s"))}",
        $"page=1",
        $"pageSize=999999",
        $"exportAll=true"
    };

            if (!string.IsNullOrEmpty(f.Shift))
                q.Add($"operationalShift={Uri.EscapeDataString(f.Shift)}");
            if (!string.IsNullOrEmpty(f.toll_Operator_ID))
                q.Add($"tollOperators={Uri.EscapeDataString(f.toll_Operator_ID)}");
            if (!string.IsNullOrEmpty(f.lane_Nr))
                q.Add($"laneNames={Uri.EscapeDataString(f.lane_Nr)}");
            if (!string.IsNullOrEmpty(f.PaymentMethod))
                q.Add($"paymentMethods={Uri.EscapeDataString(f.PaymentMethod)}");
            if (!string.IsNullOrEmpty(f.TakenAction))
                q.Add($"takenAction={Uri.EscapeDataString(f.TakenAction)}");

            string baseUrl = _configuration["BaseApiUrl:Link"];
            string endpoint = _configuration["ApiSettings:DiscrepancyReportEndpoint"];
            string url = $"{baseUrl}{endpoint}?{string.Join("&", q)}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new PageDiscrepancyModel { ExportItems = new List<DiscrepancyModel>() };

            var json = await response.Content.ReadAsStringAsync();

            var full = JsonConvert.DeserializeObject<PageDiscrepancyModel>(json);
            full.ExportItems = full.Items; // ensure not null

            return full;
        }


        // -------------------------
        // EMPTY RESULT FACTORY
        // -------------------------
        private PageDiscrepancyModel EmptyResult(DateTime start, DateTime end, int page, int pageSize)
        {
            return new PageDiscrepancyModel
            {
                Items = new List<DiscrepancyModel>(),
                totalCount = 0,
                totalPages = 0,
                page = page,
                pageSize = pageSize,
                Filters = new DiscrepancyInputModel
                {
                    StartDate = start,
                    EndDate = end,
                    Page = page,
                    PageSize = pageSize
                }
            };
        }
    }
}
