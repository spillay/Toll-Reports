using MIS.Web.Models.Discrepancy;
using Newtonsoft.Json;
using System.Globalization;

namespace MIS.Web.Services
{
    public class DiscrepancyReportService : IDiscrepancyReportService
    {
        private readonly HttpClient _httpClient;

        public DiscrepancyReportService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


       public async Task<List<DiscrepancyReportViewModel>> GetDiscrepancyDetailsAsync(
            DateTime startDate, DateTime endDate, List<string>? operationalShift, List<string>? tollOperators, List<string>? laneNames, List<string>? paymentMethods)
        
        {
            // Format dates
            string formattedStartDate = startDate.ToString("yyyy/MM/dd");
            string formattedEndDate = endDate.ToString("yyyy/MM/dd");

            string encodedStartDate = Uri.EscapeDataString(formattedStartDate);
            string encodedEndDate = Uri.EscapeDataString(formattedEndDate);


            var url = $"http://localhost:5000/discrepancy?startDate=08%2F08%2F2025&endDate=09%2F09%2F2025";

            if (operationalShift != null && operationalShift.Any())
                url += $"&operationalShift={string.Join(",", operationalShift)}";

            if (tollOperators != null && tollOperators.Any())
                url += $"&tollOperators={string.Join(",", tollOperators)}";

            if (laneNames != null && laneNames.Any())
                url += $"&laneNames={string.Join(",", laneNames)}";

            if (paymentMethods != null && paymentMethods.Any())
                url += $"&paymentMethods={string.Join(",", paymentMethods)}";


            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var settings = new JsonSerializerSettings
            {
                DateFormatString = "dd/MM/yyyy",
                Culture = CultureInfo.InvariantCulture
            };

            var discrepancys = JsonConvert.DeserializeObject<List<DiscrepancyReportViewModel>>(json, settings);

            return discrepancys ?? new List<DiscrepancyReportViewModel>();
        }
    } 
}
