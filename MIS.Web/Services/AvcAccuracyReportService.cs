using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MIS.Web.Models.AvcAccuracy;
using MIS.Web.Services.Interfaces;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MIS.Web.Services
{
    public class AvcAccuracyReportService : IAvcAccuracyReportService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<AvcAccuracyReportService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AvcAccuracyReportService(
            HttpClient httpClient,
            IConfiguration config,
            ILogger<AvcAccuracyReportService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PageAvcAccuracyReportModel> GetReportAsync(
            DateTime startDate,
            DateTime endDate,
            List<int>? shiftIds = null,
            List<int>? laneIds = null,
            List<int>? classIds = null)
        {
            var model = new PageAvcAccuracyReportModel
            {
                StartDate = startDate,
                EndDate = endDate,
                SelectedShiftIds = shiftIds ?? new List<int>(),
                SelectedLaneIds = laneIds ?? new List<int>(),
                SelectedClassIds = classIds ?? new List<int>()
            };

            try
            {
                var baseUrl = _config["BaseApiUrl:Link"];
                var detailsEndpoint = _config["ApiSettings:AvcAccuracyReportEndpoint"];
                var filterOptionsEndpoint = _config["ApiSettings:AvcAccuracyFilterOptionsEndpoint"];

                if (string.IsNullOrWhiteSpace(baseUrl))
                {
                    _logger.LogError("API base URL 'BaseApiUrl:Link' is missing from configuration.");
                    return model;
                }

                if (string.IsNullOrWhiteSpace(detailsEndpoint))
                {
                    _logger.LogError("API endpoint 'ApiSettings:AvcAccuracyReportEndpoint' is missing from configuration.");
                    return model;
                }

                if (string.IsNullOrWhiteSpace(filterOptionsEndpoint))
                {
                    _logger.LogError("API endpoint 'ApiSettings:AvcAccuracyFilterOptionsEndpoint' is missing from configuration.");
                    return model;
                }

                var filterOptionsUrl = BuildUrl(baseUrl, filterOptionsEndpoint, null);
                _logger.LogInformation("Calling AVC Accuracy filter options API: {Url}", filterOptionsUrl);

                using (var filterRequest = CreateAuthorizedGetRequest(filterOptionsUrl))
                using (var filterResponse = await _httpClient.SendAsync(filterRequest))
                {
                    if (!filterResponse.IsSuccessStatusCode)
                    {
                        _logger.LogWarning("AVC Accuracy filter options API returned status code {StatusCode}", filterResponse.StatusCode);
                    }
                    else
                    {
                        var filterJson = await filterResponse.Content.ReadAsStringAsync();

                        var filterOptions =
                            JsonConvert.DeserializeObject<AvcAccuracyFilterOptionsResponseModel>(filterJson)
                            ?? new AvcAccuracyFilterOptionsResponseModel();

                        model.ShiftOptions = filterOptions.Shifts ?? new List<AvcAccuracyFilterOptionModel>();
                        model.LaneOptions = filterOptions.Lanes ?? new List<AvcAccuracyFilterOptionModel>();
                        model.ClassOptions = filterOptions.Classes ?? new List<AvcAccuracyFilterOptionModel>();
                    }
                }

                var queryParams = BuildQueryParams(startDate, endDate, shiftIds, laneIds, classIds);
                var detailsUrl = BuildUrl(baseUrl, detailsEndpoint, queryParams);

                _logger.LogInformation("Calling AVC Accuracy details API: {Url}", detailsUrl);

                using (var detailsRequest = CreateAuthorizedGetRequest(detailsUrl))
                using (var detailsResponse = await _httpClient.SendAsync(detailsRequest))
                {
                    if (!detailsResponse.IsSuccessStatusCode)
                    {
                        _logger.LogWarning("AVC Accuracy details API returned status code {StatusCode}", detailsResponse.StatusCode);
                        return model;
                    }

                    var detailsJson = await detailsResponse.Content.ReadAsStringAsync();

                    var apiItems =
                        JsonConvert.DeserializeObject<List<AvcAccuracyApiItem>>(detailsJson)
                        ?? new List<AvcAccuracyApiItem>();

                    model.Lanes = BuildLaneRows(apiItems);
                    model.GrandTotal = BuildGrandTotal(apiItems);
                }

                return model;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error while loading AVC Accuracy report.");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON error while parsing AVC Accuracy report response.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error loading AVC Accuracy report.");
            }

            return model;
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

        private static List<string> BuildQueryParams(
            DateTime startDate,
            DateTime endDate,
            List<int>? shiftIds,
            List<int>? laneIds,
            List<int>? classIds)
        {
            var queryParams = new List<string>
            {
                $"startDate={Uri.EscapeDataString(startDate.ToString("yyyy-MM-ddTHH:mm:ss"))}",
                $"endDate={Uri.EscapeDataString(endDate.ToString("yyyy-MM-ddTHH:mm:ss"))}"
            };

            AddRepeatedQueryString(queryParams, "shiftIds", shiftIds);
            AddRepeatedQueryString(queryParams, "laneIds", laneIds);
            AddRepeatedQueryString(queryParams, "classIds", classIds);

            return queryParams;
        }

        private static void AddRepeatedQueryString(
            List<string> queryParams,
            string key,
            List<int>? values)
        {
            if (values == null || !values.Any())
                return;

            queryParams.AddRange(values.Select(x => $"{key}={x}"));
        }

        private static string BuildUrl(
            string baseUrl,
            string endpoint,
            List<string>? queryParams)
        {
            var root = baseUrl.TrimEnd('/');
            var path = endpoint.TrimStart('/');

            if (queryParams == null || queryParams.Count == 0)
                return $"{root}/{path}";

            return $"{root}/{path}?{string.Join("&", queryParams)}";
        }

        private static List<AvcAccuracyLaneRowModel> BuildLaneRows(List<AvcAccuracyApiItem> items)
        {
            return items
                .GroupBy(x => new { x.LaneId, x.LaneName })
                .OrderBy(g => g.Key.LaneId)
                .Select(group =>
                {
                    var classes = group
                        .OrderBy(x => x.DisplayOrder)
                        .Select(x => new AvcAccuracyClassCellModel
                        {
                            ClassId = x.TollClassId,
                            ClassName = x.ClassDescription,
                            DisplayOrder = x.DisplayOrder,
                            ActualCount = x.ActualCount,
                            AdjustedCount = x.AdjustedCount,
                            ActualPercentage = x.ActualPercentage,
                            AdjustedPercentage = x.AdjustedPercentage
                        })
                        .ToList();

                    var totalActual = classes.Sum(x => x.ActualCount);
                    var totalAdjusted = classes.Sum(x => x.AdjustedCount);
                    var totalClassError = Math.Abs(totalAdjusted - totalActual);

                    return new AvcAccuracyLaneRowModel
                    {
                        LaneId = group.Key.LaneId,
                        LaneName = group.Key.LaneName,
                        Classes = classes,
                        TotalActualCount = totalActual,
                        TotalAdjustedCount = totalAdjusted,
                        TotalTraffic = totalAdjusted,
                        TotalClassError = totalClassError,
                        TotalError = CalculatePercentage(totalClassError, totalAdjusted),
                        TotalAccuracyActual = CalculatePercentage(totalActual, totalAdjusted),
                        TotalAccuracyAdjusted = CalculatePercentage(totalAdjusted, totalActual)
                    };
                })
                .ToList();
        }

        private static AvcAccuracyTotalsModel BuildGrandTotal(List<AvcAccuracyApiItem> items)
        {
            var classes = items
                .GroupBy(x => new { x.TollClassId, x.ClassDescription, x.DisplayOrder })
                .OrderBy(g => g.Key.DisplayOrder)
                .Select(group =>
                {
                    var actual = group.Sum(x => x.ActualCount);
                    var adjusted = group.Sum(x => x.AdjustedCount);

                    return new AvcAccuracyClassCellModel
                    {
                        ClassId = group.Key.TollClassId,
                        ClassName = group.Key.ClassDescription,
                        DisplayOrder = group.Key.DisplayOrder,
                        ActualCount = actual,
                        AdjustedCount = adjusted,
                        ActualPercentage = CalculatePercentage(actual, adjusted),
                        AdjustedPercentage = CalculatePercentage(adjusted, actual)
                    };
                })
                .ToList();

            var totalActual = classes.Sum(x => x.ActualCount);
            var totalAdjusted = classes.Sum(x => x.AdjustedCount);
            var totalClassError = Math.Abs(totalAdjusted - totalActual);

            return new AvcAccuracyTotalsModel
            {
                Classes = classes,
                TotalActualCount = totalActual,
                TotalAdjustedCount = totalAdjusted,
                TotalTraffic = totalAdjusted,
                TotalClassError = totalClassError,
                TotalError = CalculatePercentage(totalClassError, totalAdjusted),
                TotalAccuracyActual = CalculatePercentage(totalActual, totalAdjusted),
                TotalAccuracyAdjusted = CalculatePercentage(totalAdjusted, totalActual)
            };
        }

        private static decimal CalculatePercentage(decimal numerator, decimal denominator)
        {
            if (denominator == 0)
                return 0m;

            return Math.Round((numerator * 100m) / denominator, 2);
        }
    }
}