using Microsoft.Extensions.Configuration;
using MIS.Web.Models;
using MIS.Web.Models.Transaction;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
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

        /* ============================================================
         *  HELPER: Build Query String Safely
         * ============================================================ */
        private string BuildQuery(Dictionary<string, string?> values)
        {
            var sb = new StringBuilder();

            foreach (var kv in values)
            {
                if (!string.IsNullOrWhiteSpace(kv.Value))
                {
                    sb.Append($"{kv.Key}={Uri.EscapeDataString(kv.Value)}&");
                }
            }

            return sb.ToString().TrimEnd('&');
        }



        /* ============================================================
         * 1. GET TRANSACTION DETAILS (Paginated)
         * ============================================================ */
        public async Task<PageTransactionModel> GetTransactionDetailsAsync(TransactionInputModel model)
        {
            try
            {
                string baseUrl = _config["BaseApiUrl:Link"];
                string endpoint = _config["ApiSettings:TransactionEndpoint"];

                var query = BuildQuery(new Dictionary<string, string?>
                {
                    { "startDate", model.StartDate.ToString("yyyy-MM-ddTHH:mm:ss") },
                    { "endDate",   model.EndDate.ToString("yyyy-MM-ddTHH:mm:ss") },
                    { "page",      model.page.ToString() },
                    { "pageSize",  model.pageSize.ToString() },
                    { "operationalShift", model.Shift },
                    { "tollOperators",    model.TollOperatorID },
                    { "laneNames",        model.lane_Nr },
                    { "paymentMethods",   model.PaymentMethod }
                });

                string url = $"{baseUrl}{endpoint}?{query}";

                Console.WriteLine($"[TransactionService] GET: {url}");

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
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetTransactionDetailsAsync → {ex.Message}");
                return new PageTransactionModel { items = new List<TransactionModel>() };
            }
        }

        public async Task<PageTransactionModel> GetTransactionExportAsync(TransactionInputModel model)
        {
            string baseUrl = _config["BaseApiUrl:Link"];
            string endpoint = _config["ApiSettings:TransactionEndpoint"];

            string url = $"{baseUrl}{endpoint}?startDate={model.StartDate:yyyy-MM-ddTHH:mm:ss}&endDate={model.EndDate:yyyy-MM-ddTHH:mm:ss}&exportAll=true";

            if (!string.IsNullOrEmpty(model.Shift))
                url += $"&operationalShift={Uri.EscapeDataString(model.Shift)}";

            if (!string.IsNullOrEmpty(model.TollOperatorID))
                url += $"&tollOperators={Uri.EscapeDataString(model.TollOperatorID)}";

            if (!string.IsNullOrEmpty(model.lane_Nr))
                url += $"&laneNames={Uri.EscapeDataString(model.lane_Nr)}";

            if (!string.IsNullOrEmpty(model.PaymentMethod))
                url += $"&paymentMethods={Uri.EscapeDataString(model.PaymentMethod)}";

            var response = await _httpClient.GetAsync(url);
            string json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<PageTransactionModel>(json)
                ?? new PageTransactionModel { items = new List<TransactionModel>() };
        }

        public async Task<TransactionInputModel> GetTransactionFilterOptionsAsync(TransactionInputModel model)
        {
            try
            {
                string baseUrl = _config["BaseApiUrl:Link"];
                string endpoint = _config["ApiSettings:TransactionFilterOptionsEndpoint"];

                var query = BuildQuery(new Dictionary<string, string?>
                {
                    { "startDate", model.StartDate.ToString("yyyy-MM-ddTHH:mm:ss") },
                    { "endDate",   model.EndDate.ToString("yyyy-MM-ddTHH:mm:ss") }
                });

                string url = $"{baseUrl}{endpoint}?{query}";

                Console.WriteLine($"[TransactionService] Filter Options GET: {url}");

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[WARN] Filter API returned {response.StatusCode}");
                    return new TransactionInputModel();
                }

                string json = await response.Content.ReadAsStringAsync();
                var filters = JsonConvert.DeserializeObject<TransactionInputModel>(json);

                return filters ?? new TransactionInputModel();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetTransactionFilterOptionsAsync → {ex.Message}");
                return new TransactionInputModel();
            }
        }
    }
}
