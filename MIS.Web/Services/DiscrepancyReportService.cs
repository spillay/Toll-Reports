using MIS.Web.Models.Discrepancy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MIS.Web.Services
{
    public class DiscrepancyReportService : IDiscrepancyReportService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DiscrepancyReportService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DiscrepancyReportService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<DiscrepancyReportService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PageDiscrepancyModel> GetDiscrepancyReportAsync(DiscrepancyInputModel model)
        {
            try
            {
                var url = BuildReportUrl(model, exportAll: false);

                _logger.LogInformation("Discrepancy GET: {Url}", url);

                using var request = CreateAuthorizedGetRequest(url);
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Discrepancy API failed. Status: {Status}", response.StatusCode);
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
                    _logger.LogWarning("Discrepancy API returned null/invalid JSON.");
                    return EmptyResult(model);
                }

                data.Items ??= new List<DiscrepancyModel>();
                data.ExportItems ??= new List<DiscrepancyModel>();
                data.Filters = model;

                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR: GetDiscrepancyReportAsync(model) crashed.");
                return EmptyResult(model);
            }
        }

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

        public async Task<DiscrepancyInputModel> GetDiscrepancyFilterOptionsAsync(DiscrepancyInputModel model)
        {
            try
            {
                var url = BuildFilterOptionsUrl(model);

                _logger.LogInformation("Discrepancy FilterOptions GET: {Url}", url);

                using var request = CreateAuthorizedGetRequest(url);
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("FilterOptions API failed. Status: {Status}", response.StatusCode);
                    return model;
                }

                var json = await response.Content.ReadAsStringAsync();

                var options = JsonConvert.DeserializeObject<DiscrepancyInputModel>(
                    json,
                    new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    });

                if (options == null)
                    return model;

                model.Shifts = options.Shifts ?? new List<string>();
                model.TollOperators = options.TollOperators ?? new List<string>();
                model.Lanes = options.Lanes ?? new List<string>();
                model.PaymentMethods = options.PaymentMethods ?? new List<string>();
                model.TakenActions = options.TakenActions ?? new List<string>();

                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR: GetDiscrepancyFilterOptionsAsync crashed.");
                return model;
            }
        }

        public async Task<PageDiscrepancyModel> GetFullExportAsync(DiscrepancyInputModel model)
        {
            try
            {
                var url = BuildReportUrl(model, exportAll: true);

                _logger.LogInformation("Discrepancy EXPORT GET: {Url}", url);

                using var request = CreateAuthorizedGetRequest(url);
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Export API failed. Status: {Status}", response.StatusCode);
                    return new PageDiscrepancyModel
                    {
                        Items = new List<DiscrepancyModel>(),
                        ExportItems = new List<DiscrepancyModel>(),
                        Filters = model
                    };
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
                full.ExportItems = full.Items;
                full.Filters = model;

                return full;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR: GetFullExportAsync crashed.");
                return new PageDiscrepancyModel
                {
                    Items = new List<DiscrepancyModel>(),
                    ExportItems = new List<DiscrepancyModel>(),
                    Filters = model
                };
            }
        }

        private HttpRequestMessage CreateAuthorizedGetRequest(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            AddBearerToken(request);
            return request;
        }

        private void AddBearerToken(HttpRequestMessage request)
        {
            var token = _httpContextAccessor.HttpContext?.User?.FindFirst("access_token")?.Value;

            if (string.IsNullOrWhiteSpace(token))
                throw new UnauthorizedAccessException("No JWT token found for current user.");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private string BuildReportUrl(DiscrepancyInputModel model, bool exportAll)
        {
            var baseUrl = _configuration["BaseApiUrl:Link"]?.TrimEnd('/');
            var endpoint = _configuration["ApiSettings:DiscrepancyReportEndpoint"]?.Trim();

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("BaseApiUrl:Link missing in config.");

            if (string.IsNullOrWhiteSpace(endpoint))
                throw new InvalidOperationException("ApiSettings:DiscrepancyReportEndpoint missing in config.");

            if (!endpoint.StartsWith("/"))
                endpoint = "/" + endpoint;

            var query = BuildQuery(model, exportAll);
            return $"{baseUrl}{endpoint}?{query}";
        }

        private string BuildFilterOptionsUrl(DiscrepancyInputModel model)
        {
            var baseUrl = _configuration["BaseApiUrl:Link"]?.TrimEnd('/');
            var endpoint = _configuration["ApiSettings:DiscrepancyFilterOptionsEndpoint"]?.Trim();

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("BaseApiUrl:Link missing in config.");

            if (string.IsNullOrWhiteSpace(endpoint))
                throw new InvalidOperationException("ApiSettings:DiscrepancyFilterOptionsEndpoint missing in config.");

            if (!endpoint.StartsWith("/"))
                endpoint = "/" + endpoint;

            var q = new List<string>
            {
                $"startDate={Uri.EscapeDataString(model.StartDate.ToString("yyyy-MM-ddTHH:mm:ss"))}",
                $"endDate={Uri.EscapeDataString(model.EndDate.ToString("yyyy-MM-ddTHH:mm:ss"))}"
            };

            return $"{baseUrl}{endpoint}?{string.Join("&", q)}";
        }

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
            if (values == null || values.Count == 0)
                return;

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