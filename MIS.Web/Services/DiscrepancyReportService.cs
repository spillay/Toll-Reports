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

        // ==========================================================
        // 1) Transaction-style: paginated call using model (NEW)
        // ==========================================================
        public async Task<PageDiscrepancyModel> GetDiscrepancyReportAsync(DiscrepancyInputModel model)
        {
            try
            {
                string baseUrl = _configuration["BaseApiUrl:Link"];
                string endpoint = _configuration["ApiSettings:DiscrepancyReportEndpoint"];

                string query = BuildQuery(model, exportAll: false);
                string url = $"{baseUrl}{endpoint}?{query}";

                _logger.LogInformation("📡 Discrepancy GET: {Url}", url);

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("❌ Discrepancy API failed. Status: {Status}", response.StatusCode);
                    return EmptyResult(model);
                }

                var json = await response.Content.ReadAsStringAsync();

                var data = JsonConvert.DeserializeObject<PageDiscrepancyModel>(
                    json,
                    new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    });

                if (data == null)
                {
                    _logger.LogWarning("⚠️ Discrepancy API returned NULL/invalid JSON.");
                    return EmptyResult(model);
                }

                // Keep filters for UI re-render (checked boxes)
                data.Filters = model;

                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 ERROR: GetDiscrepancyReportAsync(model) crashed.");
                return EmptyResult(model);
            }
        }

        // ==========================================================
        // 2) Backward compatible signature (OLD) - wrapper
        // ==========================================================
        public Task<PageDiscrepancyModel> GetDiscrepancyReportAsync(
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
            // Wrapper => maps old params into new checklist model
            var model = new DiscrepancyInputModel
            {
                StartDate = startDate,
                EndDate = endDate,
                Page = page,
                PageSize = pageSize,

                SelectedShifts = operationalShift ?? new List<string>(),
                SelectedTollOperators = tollOperators ?? new List<string>(),
                SelectedLanes = laneNames ?? new List<string>(),
                SelectedPaymentMethods = paymentMethods ?? new List<string>(),
                SelectedTakenActions = takenAction ?? new List<string>()
            };

            return GetDiscrepancyReportAsync(model);
        }

        // ==========================================================
        // 3) Filter options (ALL values from DB)
        //    Calls: /api/discrepancy/filter-options
        // ==========================================================
        public async Task<DiscrepancyInputModel> GetDiscrepancyFilterOptionsAsync(DiscrepancyInputModel model)
        {
            try
            {
                string baseUrl = _configuration["BaseApiUrl:Link"];
                string endpoint = _configuration["ApiSettings:DiscrepancyFilterOptionsEndpoint"];

                var q = new List<string>
                {
                    $"startDate={Uri.EscapeDataString(model.StartDate.ToString("yyyy-MM-ddTHH:mm:ss"))}",
                    $"endDate={Uri.EscapeDataString(model.EndDate.ToString("yyyy-MM-ddTHH:mm:ss"))}"
                };

                string url = $"{baseUrl}{endpoint}?{string.Join("&", q)}";

                _logger.LogInformation("📡 Discrepancy FilterOptions GET: {Url}", url);

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("❌ FilterOptions API failed. Status: {Status}", response.StatusCode);
                    return model; // keep existing model
                }

                var json = await response.Content.ReadAsStringAsync();

                // API returns DiscrepancyDto with lists; we deserialize into input model if names match,
                // OR keep it simple and deserialize into dynamic then map.
                var options = JsonConvert.DeserializeObject<DiscrepancyInputModel>(
                    json,
                    new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    });

                if (options == null) return model;

                // Update option lists ONLY (keep user selections)
                model.Shifts = options.Shifts ?? new List<string>();
                model.TollOperators = options.TollOperators ?? new List<string>();
                model.Lanes = options.Lanes ?? new List<string>();
                model.PaymentMethods = options.PaymentMethods ?? new List<string>();
                model.TakenActions = options.TakenActions ?? new List<string>();

                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 ERROR: GetDiscrepancyFilterOptionsAsync crashed.");
                return model;
            }
        }

        // ==========================================================
        // 4) Full export (exportAll=true) - same filters
        // ==========================================================
        public async Task<PageDiscrepancyModel> GetFullExportAsync(DiscrepancyInputModel model)
        {
            try
            {
                string baseUrl = _configuration["BaseApiUrl:Link"];
                string endpoint = _configuration["ApiSettings:DiscrepancyReportEndpoint"];

                string query = BuildQuery(model, exportAll: true);
                string url = $"{baseUrl}{endpoint}?{query}";

                _logger.LogInformation("📡 Discrepancy EXPORT GET: {Url}", url);

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("❌ Export API failed. Status: {Status}", response.StatusCode);
                    return new PageDiscrepancyModel { ExportItems = new List<DiscrepancyModel>(), Filters = model };
                }

                var json = await response.Content.ReadAsStringAsync();

                var full = JsonConvert.DeserializeObject<PageDiscrepancyModel>(
                    json,
                    new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    });

                full ??= new PageDiscrepancyModel();
                full.Items ??= new List<DiscrepancyModel>();

                // ExportItems should be full dataset
                full.ExportItems = full.Items;
                full.Filters = model;

                return full;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 ERROR: GetFullExportAsync crashed.");
                return new PageDiscrepancyModel { ExportItems = new List<DiscrepancyModel>(), Filters = model };
            }
        }

        // ==========================================================
        // Helpers
        // ==========================================================

        private string BuildQuery(DiscrepancyInputModel model, bool exportAll)
        {
            var q = new List<string>
            {
                $"startDate={Uri.EscapeDataString(model.StartDate.ToString("yyyy-MM-ddTHH:mm:ss"))}",
                $"endDate={Uri.EscapeDataString(model.EndDate.ToString("yyyy-MM-ddTHH:mm:ss"))}"
            };

            if (exportAll)
            {
                q.Add("exportAll=true");
                q.Add("page=1");
                q.Add($"pageSize={int.MaxValue}");
            }
            else
            {
                q.Add($"page={model.Page}");
                q.Add($"pageSize={model.PageSize}");
            }

            AddList(q, "operationalShift", model.SelectedShifts);
            AddList(q, "tollOperators", model.SelectedTollOperators);
            AddList(q, "laneNames", model.SelectedLanes);
            AddList(q, "paymentMethods", model.SelectedPaymentMethods);
            AddList(q, "takenAction", model.SelectedTakenActions);

            return string.Join("&", q);
        }

        private static void AddList(List<string> q, string key, List<string> values)
        {
            if (values == null || values.Count == 0) return;

            foreach (var v in values
                         .Where(x => !string.IsNullOrWhiteSpace(x))
                         .Select(x => x.Trim())
                         .Distinct(StringComparer.OrdinalIgnoreCase))
            {
                q.Add($"{key}={Uri.EscapeDataString(v)}");
            }
        }

        private PageDiscrepancyModel EmptyResult(DiscrepancyInputModel model)
        {
            return new PageDiscrepancyModel
            {
                Items = new List<DiscrepancyModel>(),
                totalCount = 0,
                totalPages = 0,
                page = model.Page,
                pageSize = model.PageSize,
                Filters = model,
                ExportItems = new List<DiscrepancyModel>()
            };
        }
    }
}