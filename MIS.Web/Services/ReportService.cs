using MIS.Web.Models.Transaction;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MIS.Web.Services
{
    public class ReportService : IReportService
    {
        private readonly HttpClient _httpClient;

        public ReportService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PageTransactionModel> GetTransactionDetailsAsync(
            int pageNumber,
            int pageSize,
            DateTime startDate,
            DateTime endDate,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null,
            List<string>? laneNames = null,
            List<string>? paymentMethods = null
            )
        {
            string formattedStart = startDate.ToString("s"); // yyyy-MM-ddTHH:mm:ss
            string formattedEnd = endDate.ToString("s");
            string formattedPageNumber = pageNumber.ToString();
            string formattedPageSize = pageSize.ToString();

             var url = $"http://localhost:5000/api/Transaction/details?startDate={Uri.EscapeDataString(formattedStart)}&endDate={Uri.EscapeDataString(formattedEnd)}&page={Uri.EscapeDataString(formattedPageNumber)}&pageSize={Uri.EscapeDataString(formattedPageSize)}";

            if (operationalShift != null && operationalShift.Any())
                url += $"&operationalShift={Uri.EscapeDataString(string.Join(",", operationalShift))}";

            if (tollOperators != null && tollOperators.Any())
                url += $"&tollOperators={Uri.EscapeDataString(string.Join(",", tollOperators))}";

            if (laneNames != null && laneNames.Any())
                url += $"&laneNames={Uri.EscapeDataString(string.Join(",", laneNames))}";

            if (paymentMethods != null && paymentMethods.Any())
                url += $"&paymentMethods={Uri.EscapeDataString(string.Join(",", paymentMethods))}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return new PageTransactionModel();
            }

            var json = await response.Content.ReadAsStringAsync();

            var transactions = JsonConvert.DeserializeObject<PageTransactionModel>(json);

            return transactions ?? new PageTransactionModel();
        }
    }
}
