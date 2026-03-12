using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MIS.Web.Models.DailyCashup;

namespace MIS.Web.Services
{
    public class DailyCashupReportService : IDailyCashupReportService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<DailyCashupReportService> _logger;

        public DailyCashupReportService(
            HttpClient httpClient,
            IConfiguration config,
            ILogger<DailyCashupReportService> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
        }

        // ✅ 1) MAIN REPORT DATA (ID-based multi-select filters)
        public async Task<PageDailyCashupModel> GetDailyCashupAsync(
            DateTime startDate,
            DateTime endDate,
            List<int>? shiftIds = null,
            List<long>? systemUserIds = null,
            int page = 1,
            int pageSize = 10)
        {
            // Base url + endpoint
            var baseUrl = (_config["BaseApiUrl:Link"] ?? "").TrimEnd('/');
            var endpoint = (_config["ApiSettings:DailyCashupEndpoint"] ?? "").TrimStart('/');

            //  Build querystring (repeat params for list values)
            var query = new List<string>
            {
                $"startDate={Uri.EscapeDataString(startDate.ToString("yyyy-MM-dd"))}",
                $"endDate={Uri.EscapeDataString(endDate.ToString("yyyy-MM-dd"))}",
                $"page={page}",
                $"pageSize={pageSize}"
            };

            // shiftIds=1&shiftIds=2...
            if (shiftIds != null && shiftIds.Count > 0)
            {
                foreach (var id in shiftIds)
                    query.Add($"shiftIds={Uri.EscapeDataString(id.ToString())}");
            }

            // systemUserIds=10&systemUserIds=11...
            if (systemUserIds != null && systemUserIds.Count > 0)
            {
                foreach (var id in systemUserIds)
                    query.Add($"systemUserIds={Uri.EscapeDataString(id.ToString())}");
            }

            var url = $"{baseUrl}/{endpoint}?{string.Join("&", query)}";

            try
            {
                _logger.LogInformation("➡️ Calling DailyCashup API: {Url}", url);

                using var response = await _httpClient.GetAsync(url);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("⚠️ DailyCashup returned {Code}: {Body}", response.StatusCode, body);
                    return BuildEmptyPageModel(startDate, endDate, shiftIds, systemUserIds, page, pageSize);
                }

                // The API returns: { items: [], totalCount, page, pageSize, totalPages }
                var api = JsonConvert.DeserializeObject<ApiPagedResult<DailyCashupModel>>(body)
                          ?? new ApiPagedResult<DailyCashupModel>();

                var model = new PageDailyCashupModel
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Items = api.Items ?? new List<DailyCashupModel>(),
                    SelectedShiftIds = shiftIds ?? new List<int>(),
                    SelectedSystemUserIds = systemUserIds ?? new List<long>()
                };

                //  Consistency: set pagination props ONLY if they exist on PageDailyCashupModel
                TrySet(model, "page", api.Page);
                TrySet(model, "Page", api.Page);

                TrySet(model, "pageSize", api.PageSize);
                TrySet(model, "PageSize", api.PageSize);

                TrySet(model, "totalPages", api.TotalPages);
                TrySet(model, "TotalPages", api.TotalPages);

                TrySet(model, "totalCount", api.TotalCount);
                TrySet(model, "TotalCount", api.TotalCount);

                _logger.LogInformation("✅ DailyCashup: {Count} records received", model.Items?.Count ?? 0);
                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔥 Failed to call DailyCashup endpoint");
                return BuildEmptyPageModel(startDate, endDate, shiftIds, systemUserIds, page, pageSize);
            }
        }

        // 2) ONE FILTER ENDPOINT (NOT DATE FILTERED)
        public async Task<(List<CheckItemModel<int>> Shifts, List<CheckItemModel<long>> Operators)> GetFiltersAsync()
        {
            var baseUrl = (_config["BaseApiUrl:Link"] ?? "").TrimEnd('/');
            var url = $"{baseUrl}/api/DailyCashup/filters";

            try
            {
                _logger.LogInformation("➡️ Calling DailyCashup Filters API: {Url}", url);

                using var response = await _httpClient.GetAsync(url);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("⚠️ Filters returned {Code}: {Body}", response.StatusCode, body);
                    return (new List<CheckItemModel<int>>(), new List<CheckItemModel<long>>());
                }

                var dto = JsonConvert.DeserializeObject<DailyCashupFiltersDto>(body) ?? new DailyCashupFiltersDto();

                var shifts = new List<CheckItemModel<int>>();
                foreach (var s in dto.Shifts)
                {
                    shifts.Add(new CheckItemModel<int>
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Selected = false
                    });
                }

                var ops = new List<CheckItemModel<long>>();
                foreach (var o in dto.TollOperators)
                {
                    ops.Add(new CheckItemModel<long>
                    {
                        Id = o.Id,
                        Name = o.Name,
                        Selected = false
                    });
                }

                return (shifts, ops);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔥 Exception calling /api/DailyCashup/filters");
                return (new List<CheckItemModel<int>>(), new List<CheckItemModel<long>>());
            }
        }

        // Helpers

        private static PageDailyCashupModel BuildEmptyPageModel(
            DateTime startDate,
            DateTime endDate,
            List<int>? shiftIds,
            List<long>? systemUserIds,
            int page,
            int pageSize)
        {
            var model = new PageDailyCashupModel
            {
                StartDate = startDate,
                EndDate = endDate,
                Items = new List<DailyCashupModel>(),
                SelectedShiftIds = shiftIds ?? new List<int>(),
                SelectedSystemUserIds = systemUserIds ?? new List<long>()
            };

            // Set pagination if model supports it
            TrySet(model, "page", page);
            TrySet(model, "Page", page);

            TrySet(model, "pageSize", pageSize);
            TrySet(model, "PageSize", pageSize);

            TrySet(model, "totalPages", 0);
            TrySet(model, "TotalPages", 0);

            TrySet(model, "totalCount", 0);
            TrySet(model, "TotalCount", 0);

            return model;
        }

        private static void TrySet(object target, string propName, object value)
        {
            if (target == null) return;

            var prop = target.GetType().GetProperty(propName);
            if (prop == null || !prop.CanWrite) return;

            try
            {
                // Handle nullable conversions
                var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                var safeValue = Convert.ChangeType(value, targetType);
                prop.SetValue(target, safeValue);
            }
            catch
            {
              
            }
        }

        private class ApiPagedResult<T>
        {
            [JsonProperty("items")]
            public List<T>? Items { get; set; }

            [JsonProperty("totalCount")]
            public int TotalCount { get; set; }

            [JsonProperty("page")]
            public int Page { get; set; }

            [JsonProperty("pageSize")]
            public int PageSize { get; set; }

            [JsonProperty("totalPages")]
            public int TotalPages { get; set; }
        }

        private class DailyCashupFiltersDto
        {
            [JsonProperty("shifts")]
            public List<FilterItemDto<int>> Shifts { get; set; } = new();

            [JsonProperty("tollOperators")]
            public List<FilterItemDto<long>> TollOperators { get; set; } = new();
        }

        private class FilterItemDto<T>
        {
            [JsonProperty("id")]
            public T Id { get; set; } = default!;

            [JsonProperty("name")]
            public string Name { get; set; } = "";
        }
    }
}