using MIS.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace MIS.Web.Services
{
    public class ReportService : IReportService
    {
        private readonly HttpClient _httpClient;

        public ReportService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<TransactionReportViewModel>> GetTransactionDetailsAsync(
            DateTime sDate,
            DateTime eDate,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null,
            List<string>? laneNames = null,
            List<string>? paymentMethods = null)

        {

            string formattedsDate = sDate.ToString("yyyy/MM/dd");
            string formattedeDate = eDate.ToString("yyyy/MM/dd");

            string encodedsDate = Uri.EscapeDataString(formattedsDate);
            string encodedeDate = Uri.EscapeDataString(formattedeDate);

            //var url = $"http://localhost:5000/api/Transaction/details?startDate="+encodedsDate+"&endDate="+encodedeDate;
            var url = $"http://localhost:5000/api/Transaction/details?startDate=2025%2F08%2F19&endDate=2025%2F08%2F22";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var settings = new JsonSerializerSettings
            {
                DateFormatString = "dd/MM/yyyy",
                Culture = CultureInfo.InvariantCulture
            };

            var transactions = JsonConvert.DeserializeObject<List<TransactionReportViewModel>>(json, settings);

            return transactions;
        }
    }
}
