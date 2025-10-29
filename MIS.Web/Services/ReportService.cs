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
        private readonly IConfiguration _configuration;

        public ReportService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        // Fetch transaction data with correct parameter names
        public async Task<PageTransactionModel> GetTransactionDetailsAsync(TransactionInputModel model)
        {
            string baseUrl = _configuration["BaseApiUrl:Link"];
            string endpoint = _configuration["ApiSettings:TransactionEndpoint"];

            // Build query
            string url = $"{baseUrl}{endpoint}?startDate={model.StartDate:yyyy-MM-ddTHH:mm:ss}&endDate={model.EndDate:yyyy-MM-ddTHH:mm:ss}&page={model.page}&pageSize={model.pageSize}";

            // Match backend parameter names
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
                Console.WriteLine($"[WARN] Transaction API returned {response.StatusCode}");
                return new PageTransactionModel { items = new List<TransactionModel>() };
            }

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<PageTransactionModel>(json);
            return data ?? new PageTransactionModel { items = new List<TransactionModel>() };
        }

        //  Fetch dropdown filter options
        public async Task<FilterOptionsModel> GetTransactionFilterOptionsAsync(TransactionInputModel model)
        {
            string baseUrl = _configuration["BaseApiUrl:Link"];
            string endpoint = _configuration["ApiSettings:TransactionFilterOptionsEndpoint"];
            string url = $"{baseUrl}{endpoint}?startDate={model.StartDate:yyyy-MM-ddTHH:mm:ss}&endDate={model.EndDate:yyyy-MM-ddTHH:mm:ss}";

            Console.WriteLine($"[DEBUG] Fetching filter options from: {url}");

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[WARN] Filter options API returned {response.StatusCode}");
                return new FilterOptionsModel();
            }

            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[DEBUG] Filter Options JSON => {json}");

            var filters = JsonConvert.DeserializeObject<FilterOptionsModel>(json);
            return filters ?? new FilterOptionsModel();
        }
    }
}
