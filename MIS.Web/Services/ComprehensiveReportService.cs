using MIS.Web.Models.Comprehensive;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MIS.Web.Services
{
    public class ComprehensiveReportService : IComprehensiveReportService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ComprehensiveReportService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<List<ComprehensiveModel>> GetComprehensiveDetailsAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null,
            List<string>? laneNames = null,
            List<string>? paymentMethods = null,
            List<string>? laneDiscountTypes = null,
            List<string>? classification = null,
            List<string>? transactionTypes = null)
        {
           // var baseUrl = _configuration["ApiSettings:ComprehensiveReportApiUrl"];
           

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
            AddIfAny("laneDiscountTypes", laneDiscountTypes);
            AddIfAny("classification", classification);
            AddIfAny("transactionTypes", transactionTypes);

            string baseUrl = _configuration["BaseApiUrl:Link"];
            string endpoint = _configuration["ApiSettings:ComprehensiveReportEndpoint"];
           // string url = $"{baseUrl}{endpoint}?{string.Join("&", queryParams)}";
            string url = $"{baseUrl}{endpoint}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var settings = new JsonSerializerSettings
            {
                DateFormatString = "dd/MM/yyyy",
                Culture = CultureInfo.InvariantCulture
            };

            var result = JsonConvert.DeserializeObject<List<ComprehensiveModel>>(content, settings);
            return result ?? new List<ComprehensiveModel>();
        }
    }
}
