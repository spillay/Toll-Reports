using MIS.Web.Models.Traffic.Hourly;
using Newtonsoft.Json;
using System.Net.Http;
using Toll.Reporting.Api.DTOs;

namespace MIS.Web.Services
{
    public class HourlyTrafficReportService : IHourlyTrafficReportService
    {
        private readonly HttpClient _httpClient;

        public HourlyTrafficReportService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Gets the hourly traffic report for the given date range, classifications, shifts, and operational day flag.
        /// </summary>
        public async Task<PageHourlyTrafficModel> GetTrafficReportAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? classifications = null,
            List<int>? shifts = null,
            bool operationalDay = false)
        {
            // Base URL
            var url = $"http://localhost:5000/api/HourlyTraffic/GetHourlyTrafficByDate" +
                      $"?startDate={startDate:MM/dd/yyyy}" +
                      $"&endDate={endDate:MM/dd/yyyy}" +
                      $"&operationalDay={operationalDay.ToString().ToLower()}";

            // Add classifications (API expects a single comma-separated string)
            if (classifications != null && classifications.Any())
            {
                url += $"&classification={Uri.EscapeDataString(string.Join(",", classifications))}";
            }

            // Add shifts (API expects multiple shift query parameters, e.g., &shifts=1&shifts=2)
            
            if (shifts != null && shifts.Any())
            {
                foreach (var shift in shifts)
                {
                    url += $"&shifts={shift}";
                }
            }


            // Call API
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new PageHourlyTrafficModel(); // empty model if API fails

            var json = await response.Content.ReadAsStringAsync();
            var apiResult = JsonConvert.DeserializeObject<List<HourlyTrafficDto>>(json);

            // Map API DTOs to front-end models
            return new PageHourlyTrafficModel
            {
                Items = apiResult?.Select(x => new HourlyTrafficModel
                {
                    Period = x.StartDate,
                    Classification = x.Classification,
                    Count = x.Count
                }).ToList() ?? new List<HourlyTrafficModel>()
            };
        }
    }
}
