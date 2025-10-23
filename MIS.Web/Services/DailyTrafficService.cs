using MIS.Web.Models.Traffic.Daily;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MIS.Web.Services
{
    public class DailyTrafficReportService : IDailyTrafficReportService
    {
        private readonly HttpClient _httpClient;

        public DailyTrafficReportService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PageDailyTrafficModel> GetTrafficReportAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? classifications = null,
            List<int>? shifts = null,
            bool operationalDay = false)
        {
            // Use your API base URL - make sure this matches your deployed API address
            string baseUrl = "http://localhost:5000/api/DailyTraffic";

            // Use ISO-like date format (yyyy-MM-dd) to avoid culture/parsing issues
            var queryParams = new List<string>
            {
                $"startDate={Uri.EscapeDataString(startDate.ToString("yyyy-MM-dd"))}",
                $"endDate={Uri.EscapeDataString(endDate.ToString("yyyy-MM-dd"))}",
                $"operationalDay={operationalDay.ToString().ToLower()}"
            };

            // Add classification (single value). If you want multiple, change this to join by comma.
            if (classifications?.Count > 0)
            {
                // API expects comma-separated classification string if you decide to pass multiple
                queryParams.Add($"classification={Uri.EscapeDataString(classifications.First())}");
            }

            // CRITICAL FIX:
            // API controller expects `shifts` as a comma-separated string (e.g. shifts=1,2)
            // rather than multiple `shifts=` keys. So we build one param when shifts present.
            if (operationalDay && shifts?.Count > 0)
            {
                var joined = string.Join(",", shifts);
                queryParams.Add($"shifts={Uri.EscapeDataString(joined)}");
            }

            string url = $"{baseUrl}?{string.Join("&", queryParams)}";

            Console.WriteLine($"🟩 DailyTraffic API URL: {url}");
            Console.WriteLine($"🟩 Service - OperationalDay: {operationalDay}");
            Console.WriteLine($"🟩 Service - Shifts: {(shifts != null ? string.Join(",", shifts) : "null")}");

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ API Error: {response.StatusCode} - {response.ReasonPhrase}");
                    return CreateEmptyModel(startDate, endDate, classifications, shifts, operationalDay);
                }

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"🟦 API Response: {json}");

                var items = JsonConvert.DeserializeObject<List<DailyTrafficModel>>(json) ?? new List<DailyTrafficModel>();

                return new PageDailyTrafficModel
                {
                    Items = items,
                    Filters = new DailyTrafficInputModel
                    {
                        StartDate = startDate,
                        EndDate = endDate,
                        Classification = classifications?.FirstOrDefault(),
                        Shifts = shifts ?? new List<int>(),
                        OperationalDay = operationalDay
                    },
                    // This will be replaced/populated by controller later, but keep defaults
                    Classifications = new List<string> { "Class 1", "Class 2", "Class 4", "Class M" }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error calling DailyTraffic API: {ex.Message}");
                return CreateEmptyModel(startDate, endDate, classifications, shifts, operationalDay);
            }
        }

        private PageDailyTrafficModel CreateEmptyModel(
            DateTime startDate,
            DateTime endDate,
            List<string>? classifications,
            List<int>? shifts,
            bool operationalDay)
        {
            return new PageDailyTrafficModel
            {
                Items = new List<DailyTrafficModel>(),
                Filters = new DailyTrafficInputModel
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Classification = classifications?.FirstOrDefault(),
                    Shifts = shifts ?? new List<int>(),
                    OperationalDay = operationalDay
                },
                Classifications = new List<string> { "Class 1", "Class 2", "Class 4", "Class M" }
            };
        }
    }
}
