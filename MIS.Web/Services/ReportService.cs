using Microsoft.Extensions.Configuration;
using MIS.Web.Models;
using MIS.Web.Models.Transaction;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
         * HELPER: Append single query parameter
         * ============================================================ */
        private void AppendQueryParam(StringBuilder sb, string key, string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (sb.Length > 0)
                    sb.Append("&");

                sb.Append($"{key}={Uri.EscapeDataString(value)}");
            }
        }

        /* ============================================================
         * HELPER: Append repeated query parameters for multi-select filters
         * Example:
         * operationalShift=Day&operationalShift=Night
         * ============================================================ */
        private void AppendQueryList(StringBuilder sb, string key, IEnumerable<string>? values)
        {
            if (values == null) return;

            foreach (var value in values.Where(v => !string.IsNullOrWhiteSpace(v)))
            {
                if (sb.Length > 0)
                    sb.Append("&");

                sb.Append($"{key}={Uri.EscapeDataString(value)}");
            }
        }

        /* ============================================================
         * HELPER: Build transaction query string
         * ============================================================ */
        private string BuildTransactionQuery(TransactionInputModel model, bool exportAll = false)
        {
            var sb = new StringBuilder();

            AppendQueryParam(sb, "startDate", model.StartDate.ToString("yyyy-MM-ddTHH:mm:ss"));
            AppendQueryParam(sb, "endDate", model.EndDate.ToString("yyyy-MM-ddTHH:mm:ss"));
            AppendQueryParam(sb, "page", model.page.ToString());
            AppendQueryParam(sb, "pageSize", model.pageSize.ToString());

            if (exportAll)
                AppendQueryParam(sb, "exportAll", "true");

            // Use selected filter values, not filter option lists
            AppendQueryList(sb, "operationalShift", model.SelectedShifts);
            AppendQueryList(sb, "tollOperators", model.SelectedTollOperators);
            AppendQueryList(sb, "laneNames", model.SelectedLanes);
            AppendQueryList(sb, "paymentMethods", model.SelectedPaymentMethods);

            return sb.ToString();
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

                string query = BuildTransactionQuery(model);
                string url = $"{baseUrl}{endpoint}?{query}";

                Console.WriteLine($"[TransactionService] GET: {url}");

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[WARN] Transaction API returned {response.StatusCode}");
                    return new PageTransactionModel { items = new List<TransactionModel>() };
                }

                string json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<PageTransactionModel>(json);

                return data ?? new PageTransactionModel { items = new List<TransactionModel>() };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetTransactionDetailsAsync → {ex.Message}");
                return new PageTransactionModel { items = new List<TransactionModel>() };
            }
        }

        /* ============================================================
         * 2. GET TRANSACTION EXPORT (All rows)
         * ============================================================ */
        public async Task<PageTransactionModel> GetTransactionExportAsync(TransactionInputModel model)
        {
            try
            {
                string baseUrl = _config["BaseApiUrl:Link"];
                string endpoint = _config["ApiSettings:TransactionEndpoint"];

                string query = BuildTransactionQuery(model, exportAll: true);
                string url = $"{baseUrl}{endpoint}?{query}";

                Console.WriteLine($"[TransactionService] EXPORT GET: {url}");

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[WARN] Transaction export API returned {response.StatusCode}");
                    return new PageTransactionModel { items = new List<TransactionModel>() };
                }

                string json = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<PageTransactionModel>(json)
                       ?? new PageTransactionModel { items = new List<TransactionModel>() };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetTransactionExportAsync → {ex.Message}");
                return new PageTransactionModel { items = new List<TransactionModel>() };
            }
        }

        /* ============================================================
         * 3. GET TRANSACTION FILTER OPTIONS
         * ============================================================ */
        public async Task<TransactionInputModel> GetTransactionFilterOptionsAsync(TransactionInputModel model)
        {
            try
            {
                string baseUrl = _config["BaseApiUrl:Link"];
                string endpoint = _config["ApiSettings:TransactionFilterOptionsEndpoint"];

                var sb = new StringBuilder();
                AppendQueryParam(sb, "startDate", model.StartDate.ToString("yyyy-MM-ddTHH:mm:ss"));
                AppendQueryParam(sb, "endDate", model.EndDate.ToString("yyyy-MM-ddTHH:mm:ss"));

                string url = $"{baseUrl}{endpoint}?{sb}";

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