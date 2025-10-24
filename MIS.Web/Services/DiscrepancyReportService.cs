using MIS.Web.Models.Discrepancy;
using Newtonsoft.Json;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MIS.Web.Services
{
    public class DiscrepancyReportService : IDiscrepancyReportService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public DiscrepancyReportService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<List<DiscrepancyReportViewModel>> GetDiscrepancyDetailsAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null,
            List<string>? laneNames = null,
            List<string>? paymentMethods = null)
        {
            // Read base URL from configuration
            var baseUrl = _configuration["ApiSettings:DiscrepancyReportApiUrl"];
            if (string.IsNullOrEmpty(baseUrl))
                throw new InvalidOperationException("DiscrepancyReportApiUrl is not configured in appsettings.json.");

            // Encode dates
            string encodedStartDate = Uri.EscapeDataString(startDate.ToString("yyyy/MM/dd"));
            string encodedEndDate = Uri.EscapeDataString(endDate.ToString("yyyy/MM/dd"));

            var queryParts = new List<string>
            {
                $"startDate={encodedStartDate}",
                $"endDate={encodedEndDate}"
            };

            void AddIfAny(string key, List<string>? list)
            {
                if (list != null && list.Any())
                    queryParts.Add($"{key}={Uri.EscapeDataString(string.Join(",", list))}");
            }

            AddIfAny("operationalShift", operationalShift);
            AddIfAny("tollOperators", tollOperators);
            AddIfAny("laneNames", laneNames);
            AddIfAny("paymentMethods", paymentMethods);

            var url = $"{baseUrl}?{string.Join("&", queryParts)}";

            Console.WriteLine("Request URL: " + url); // Optional debug

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var settings = new JsonSerializerSettings
            {
                DateFormatString = "dd/MM/yyyy",
                Culture = CultureInfo.InvariantCulture
            };

            var result = JsonConvert.DeserializeObject<List<DiscrepancyReportViewModel>>(json, settings);

            return result ?? new List<DiscrepancyReportViewModel>();
        }
    }
}
