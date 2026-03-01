using MIS.Web.Models.Traffic.Hourly;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace MIS.Web.Services
{
    public class HourlyTrafficReportService : IHourlyTrafficReportService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HourlyTrafficReportService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<List<string>> GetAllClassificationsAsync()
        {
            var baseUrl = _configuration["BaseApiUrl:Link"] ?? "";
            baseUrl = baseUrl.TrimEnd('/'); 

            var url = $"{baseUrl}/api/HourlyTraffic/GetAllClassifications";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return new List<string>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();
        }

        public async Task<PageHourlyTrafficModel> GetTrafficReportAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? classifications = null,
            List<int>? shifts = null,
            bool operationalDay = false)
        {
            var queryParams = new List<string>
            {
                $"startDate={Uri.EscapeDataString(startDate.ToString("MM/dd/yyyy"))}",
                $"endDate={Uri.EscapeDataString(endDate.ToString("MM/dd/yyyy"))}",
                $"operationalDay={operationalDay.ToString().ToLower()}"
            };

            if (classifications?.Any() == true)
                queryParams.Add($"classification={Uri.EscapeDataString(string.Join(",", classifications))}");

            if (shifts?.Any() == true)
                queryParams.Add($"shifts={Uri.EscapeDataString(string.Join(",", shifts))}");

            string baseUrl = _configuration["BaseApiUrl:Link"];
            string endpoint = _configuration["ApiSettings:HourlyTrafficEndpoint"];
            string url = $"{baseUrl}{endpoint}?{string.Join("&", queryParams)}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return new PageHourlyTrafficModel();

                var json = await response.Content.ReadAsStringAsync();
                var apiResult = JsonConvert.DeserializeObject<List<HourlyTrafficModel>>(json);

                return new PageHourlyTrafficModel
                {
                    Items = apiResult?.Select(x => new HourlyTrafficModel
                    {
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        Classification = x.Classification,
                        Count = x.Count
                    }).ToList() ?? new List<HourlyTrafficModel>()
                };
            }
            catch
            {
                return new PageHourlyTrafficModel();
            }
        }

    }
}