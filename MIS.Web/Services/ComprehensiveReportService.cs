using MIS.Web.Models.Comprehensive;
using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MIS.Web.Services
{
    public class ComprehensiveReportService : IComprehensiveReportService
    {
        private readonly HttpClient _httpClient;

        public ComprehensiveReportService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Calls the backend API (Toll.Reporting.Api) and passes optional filters as comma-separated query parameters.
        /// </summary>
        public async Task<List<ComprehensiveReportViewModel>> GetComprehensiveDetailsAsync(
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
            string formattedStartDate = startDate.ToString("yyyy/MM/dd");
            string formattedEndDate = endDate.ToString("yyyy/MM/dd");
            string encodedStartDate = Uri.EscapeDataString(formattedStartDate);
            string encodedEndDate = Uri.EscapeDataString(formattedEndDate);

            var queryParts = new List<string>
            {
                $"startDate={encodedStartDate}",
                $"endDate={encodedEndDate}"
            };

            // Helper to append list as comma separated values
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

            var url = $"http://localhost:5000/report?{string.Join("&", queryParts)}";

            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            // debug log
            Console.WriteLine("API Response length: " + (content?.Length ?? 0));

            response.EnsureSuccessStatusCode();

            var settings = new JsonSerializerSettings
            {
                DateFormatString = "dd/MM/yyyy",
                Culture = CultureInfo.InvariantCulture
            };

            var comprehensives = JsonConvert.DeserializeObject<List<ComprehensiveReportViewModel>>(content, settings);

            return comprehensives ?? new List<ComprehensiveReportViewModel>();
        }
    }
}
