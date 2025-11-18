using Microsoft.Extensions.Configuration;
using MIS.Web.Models;
using MIS.Web.Models.Transaction;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MIS.Web.Services
{
    public class ReportService : IReportService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public ReportService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        // ✅ Fetch paginated transaction data
        public async Task<PageTransactionModel> GetTransactionDetailsAsync(TransactionInputModel model)
        {
            string baseUrl = _config["BaseApiUrl:Link"];
            string endpoint = _config["ApiSettings:TransactionEndpoint"];
            string url = $"{baseUrl}{endpoint}?startDate={model.StartDate:yyyy-MM-ddTHH:mm:ss}&endDate={model.EndDate:yyyy-MM-ddTHH:mm:ss}&page={model.page}&pageSize={model.pageSize}";

            if (!string.IsNullOrEmpty(model.Shift))
                url += $"&operationalShift={Uri.EscapeDataString(model.Shift)}";
            if (!string.IsNullOrEmpty(model.TollOperatorID))
                url += $"&tollOperators={Uri.EscapeDataString(model.TollOperatorID)}";
            if (!string.IsNullOrEmpty(model.lane_Nr))
                url += $"&laneNames={Uri.EscapeDataString(model.lane_Nr)}";
            if (!string.IsNullOrEmpty(model.PaymentMethod))
                url += $"&paymentMethods={Uri.EscapeDataString(model.PaymentMethod)}";

            Console.WriteLine($"[DEBUG] Transaction API URL => {url}");

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[WARN] API returned {response.StatusCode}");
                return new PageTransactionModel { items = new List<TransactionModel>() };
            }

            string json = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<PageTransactionModel>(json);
            return data ?? new PageTransactionModel { items = new List<TransactionModel>() };
        }

        // ✅ Fetch dropdown filters — now uses TransactionInputModel (no separate FilterOptionsModel)
        public async Task<TransactionInputModel> GetTransactionFilterOptionsAsync(TransactionInputModel model)
        {
            string baseUrl = _config["BaseApiUrl:Link"];
            string endpoint = _config["ApiSettings:TransactionFilterOptionsEndpoint"];
            string url = $"{baseUrl}{endpoint}?startDate={model.StartDate:yyyy-MM-ddTHH:mm:ss}&endDate={model.EndDate:yyyy-MM-ddTHH:mm:ss}";

            Console.WriteLine($"[DEBUG] Filter Options API URL => {url}");

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[WARN] Filter Options API returned {response.StatusCode}");
                return new TransactionInputModel();
            }

            string json = await response.Content.ReadAsStringAsync();
            var filters = JsonConvert.DeserializeObject<TransactionInputModel>(json);
            return filters ?? new TransactionInputModel();
        }
    }
}
