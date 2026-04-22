using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DailyCashupReportService(
            HttpClient httpClient,
            IConfiguration config,
            ILogger<DailyCashupReportService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PageDailyCashupModel> GetDailyCashupAsync(
            DateTime startDate,
            DateTime endDate,
            List<int>? shiftIds = null,
            List<long>? systemUserIds = null,
            int page = 1,
            int pageSize = 10)
        {
            var baseUrl = (_config["BaseApiUrl:Link"] ?? string.Empty).TrimEnd('/');
            var endpoint = (_config["ApiSettings:DailyCashupEndpoint"] ?? string.Empty).TrimStart('/');

            var query = new List<string>
            {
                $"startDate={Uri.EscapeDataString(startDate.ToString("yyyy-MM-dd"))}",
                $"endDate={Uri.EscapeDataString(endDate.ToString("yyyy-MM-dd"))}",
                $"page={page}",
                $"pageSize={pageSize}"
            };

            if (shiftIds != null && shiftIds.Count > 0)
            {
                foreach (var id in shiftIds)
                    query.Add($"shiftIds={Uri.EscapeDataString(id.ToString())}");
            }

            if (systemUserIds != null && systemUserIds.Count > 0)
            {
                foreach (var id in systemUserIds)
                    query.Add($"systemUserIds={Uri.EscapeDataString(id.ToString())}");
            }

            var url = $"{baseUrl}/{endpoint}?{string.Join("&", query)}";

            try
            {
                _logger.LogInformation("Calling DailyCashup API: {Url}", url);

                using var request = CreateAuthorizedGetRequest(url);
                using var response = await _httpClient.SendAsync(request);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("DailyCashup returned {Code}: {Body}", response.StatusCode, body);
                    return BuildEmptyPageModel(startDate, endDate, shiftIds, systemUserIds, page, pageSize);
                }

                var api = JsonConvert.DeserializeObject<ApiDailyCashupResultDto>(body)
                          ?? new ApiDailyCashupResultDto();

                var model = new PageDailyCashupModel
                {
                    StartDate = startDate,
                    EndDate = endDate,

                    Items = api.Items ?? new List<DailyCashupModel>(),
                    FullItems = api.FullItems ?? new List<DailyCashupModel>(),
                    ShiftTotals = api.ShiftTotals ?? new List<DailyCashupShiftTotalModel>(),
                    GrandTotal = api.GrandTotal ?? new DailyCashupGrandTotalModel(),

                    SelectedShiftIds = shiftIds ?? new List<int>(),
                    SelectedSystemUserIds = systemUserIds ?? new List<long>(),

                    page = api.Page,
                    pageSize = api.PageSize,
                    totalPages = api.TotalPages,
                    totalCount = api.TotalCount
                };

                _logger.LogInformation(
                    "DailyCashup loaded successfully. Items={ItemCount}, FullItems={FullCount}, ShiftTotals={ShiftTotalCount}",
                    model.Items.Count,
                    model.FullItems.Count,
                    model.ShiftTotals.Count);

                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to call DailyCashup endpoint");
                return BuildEmptyPageModel(startDate, endDate, shiftIds, systemUserIds, page, pageSize);
            }
        }

        public async Task<(List<CheckItemModel<int>> Shifts, List<CheckItemModel<long>> Operators)> GetFiltersAsync()
        {
            var baseUrl = (_config["BaseApiUrl:Link"] ?? string.Empty).TrimEnd('/');
            var filtersEndpoint = (_config["ApiSettings:DailyCashupFiltersEndpoint"] ?? "api/DailyCashup/filters").TrimStart('/');

            var url = $"{baseUrl}/{filtersEndpoint}";

            try
            {
                _logger.LogInformation("Calling DailyCashup Filters API: {Url}", url);

                using var request = CreateAuthorizedGetRequest(url);
                using var response = await _httpClient.SendAsync(request);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("DailyCashup filters returned {Code}: {Body}", response.StatusCode, body);
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

                var operators = new List<CheckItemModel<long>>();
                foreach (var o in dto.TollOperators)
                {
                    operators.Add(new CheckItemModel<long>
                    {
                        Id = o.Id,
                        Name = o.Name,
                        Selected = false
                    });
                }

                return (shifts, operators);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception calling DailyCashup filters endpoint");
                return (new List<CheckItemModel<int>>(), new List<CheckItemModel<long>>());
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

        private static PageDailyCashupModel BuildEmptyPageModel(
            DateTime startDate,
            DateTime endDate,
            List<int>? shiftIds,
            List<long>? systemUserIds,
            int page,
            int pageSize)
        {
            return new PageDailyCashupModel
            {
                StartDate = startDate,
                EndDate = endDate,

                Items = new List<DailyCashupModel>(),
                FullItems = new List<DailyCashupModel>(),
                ShiftTotals = new List<DailyCashupShiftTotalModel>(),
                GrandTotal = new DailyCashupGrandTotalModel(),

                SelectedShiftIds = shiftIds ?? new List<int>(),
                SelectedSystemUserIds = systemUserIds ?? new List<long>(),

                page = page,
                pageSize = pageSize,
                totalPages = 0,
                totalCount = 0
            };
        }

        private class ApiDailyCashupResultDto
        {
            [JsonProperty("fullItems")]
            public List<DailyCashupModel>? FullItems { get; set; }

            [JsonProperty("items")]
            public List<DailyCashupModel>? Items { get; set; }

            [JsonProperty("shiftTotals")]
            public List<DailyCashupShiftTotalModel>? ShiftTotals { get; set; }

            [JsonProperty("grandTotal")]
            public DailyCashupGrandTotalModel? GrandTotal { get; set; }

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
            public string Name { get; set; } = string.Empty;
        }
    }
}