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
        private readonly IConfiguration _configuration;

        public DailyTrafficReportService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<PageDailyTrafficModel> GetTrafficReportAsync(
        DateTime startDate,
        DateTime endDate,
        List<string>? classifications = null,
        List<int>? shifts = null,
        bool operationalDay = false)
            {
                // Read from appsettings.json instead of hardcoding
                //string baseUrl = _configuration["ApiSettings:DailyTrafficApiUrl"];

                var queryParams = new List<string>
        {
            $"startDate={Uri.EscapeDataString(startDate.ToString("yyyy-MM-dd"))}",
            $"endDate={Uri.EscapeDataString(endDate.ToString("yyyy-MM-dd"))}",
            $"operationalDay={operationalDay.ToString().ToLower()}"
        };
            

            if (classifications?.Count > 0)
                {
                    queryParams.Add($"classification={Uri.EscapeDataString(classifications.First())}");
                }

                if (operationalDay && shifts?.Count > 0)
                {
                    var joined = string.Join(",", shifts);
                    queryParams.Add($"shifts={Uri.EscapeDataString(joined)}");
                }

            // string url = $"{baseUrl}?{string.Join("&", queryParams)}";
            string baseUrl = _configuration["BaseApiUrl:Link"];
            string endpoint = _configuration["ApiSettings:DailyTrafficEndpoint"];
            string url = $"{baseUrl}{endpoint}?{string.Join("&", queryParams)}";


            try
            {
                    var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    {
                        return CreateEmptyModel(startDate, endDate, classifications, shifts, operationalDay);
                    }

                    var json = await response.Content.ReadAsStringAsync();

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
                  
                        Classifications = new List<string> { "Class 1", "Class 2", "Class 4", "Class M" }
                    };
                }
                catch (Exception ex)
                {
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
