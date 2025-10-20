using MIS.Web.Models.Traffic;
using Newtonsoft.Json;
using Toll.Reporting.Api.DTOs;

namespace MIS.Web.Services
{
    public class TrafficReportService : ITrafficReportService
    {
        private readonly HttpClient _httpClient;

        public TrafficReportService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PageTrafficModel> GetTrafficReportAsync(
     int pageNumber, int pageSize, DateTime startDate, DateTime endDate, List<string>? classification = null)
        {
            var url = $"http://localhost:5000/api/Traffic/GetTraffic?" +
                      $"startDate={Uri.EscapeDataString(startDate.ToString("s"))}" +
                      $"&endDate={Uri.EscapeDataString(endDate.ToString("s"))}" +
                      $"&page={pageNumber}&pageSize={pageSize}";

            if (classification != null && classification.Any())
                url += $"&classification={Uri.EscapeDataString(string.Join(",", classification))}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new PageTrafficModel();

            var json = await response.Content.ReadAsStringAsync();

            // Deserialize API response into a DTO container
            var apiResult = JsonConvert.DeserializeObject<PagedResult<TrafficDto>>(json);
            if (apiResult == null)
                return new PageTrafficModel();

            // ✅ Map TrafficDto -> TrafficModel to ensure Date is populated
            var pageModel = new PageTrafficModel
            {
                Items = apiResult.Items?.Select(x => new TrafficModel
                {
                    Date = x.Period,              // <- Map Period to Date
                    Classification = x.Classification,
                    Count = x.Count
                }).ToList() ?? new List<TrafficModel>(),

                page = pageNumber,
                pageSize = pageSize,
                totalCount = apiResult.TotalCount,
                totalPages = (int)Math.Ceiling((double)(apiResult.TotalCount) / pageSize)
            };

            return pageModel;
        }
    }
}
