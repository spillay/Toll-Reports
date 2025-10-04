using MIS.Web.Models.Comprehensive;
using MIS.Web.Models.Discrepancy;
using Newtonsoft.Json;
using System.Globalization;

namespace MIS.Web.Services
{
    public class ComprehensiveReportService : IComprehensiveReportService
    {
        private readonly HttpClient _httpClient;

        public ComprehensiveReportService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<List<ComprehensiveReportViewModel>> GetComprehensiveDetailsAsync(
        DateTime startDate, DateTime endDate,
        List<string>? operationalShift = null,
        List<string>? tollOperators = null,
        List<string>? laneNames = null,
        List<string>? paymentMethods = null)
        {
            string formattedStartDate = startDate.ToString("yyyy/MM/dd");
            string formattedEndDate = endDate.ToString("yyyy/MM/dd");
            string encodedStartDate = Uri.EscapeDataString(formattedStartDate);
            string encodedEndDate = Uri.EscapeDataString(formattedEndDate);

            var url = $"http://localhost:5000/report?startDate={encodedStartDate}&endDate={encodedEndDate}";

            if (operationalShift != null && operationalShift.Any())
                url += $"&operationalShift={string.Join(",", operationalShift)}";

            if (tollOperators != null && tollOperators.Any())
                url += $"&tollOperators={string.Join(",", tollOperators)}";

            if (laneNames != null && laneNames.Any())
                url += $"&laneNames={string.Join(",", laneNames)}";

            if (paymentMethods != null && paymentMethods.Any())
                url += $"&paymentMethods={string.Join(",", paymentMethods)}";

            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("API Response: " + content);

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
