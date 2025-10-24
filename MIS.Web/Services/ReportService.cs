using MIS.Web.Models;
using MIS.Web.Models.Transaction;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MIS.Web.Services
{
    public class ReportService : IReportService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ReportService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<TransactionInputModel> GetTransactionDetailsAsync(TransactionInputModel model)
        {
            // Read API base URL from configuration
            var baseUrl = _configuration["ApiSettings:TransactionApiUrl"];
            if (string.IsNullOrEmpty(baseUrl))
                throw new InvalidOperationException("TransactionApiUrl is not configured in appsettings.json.");

            string start = Uri.EscapeDataString(model.StartDate.ToString("s"));
            string end = Uri.EscapeDataString(model.EndDate.ToString("s"));

            var queryParts = new System.Collections.Generic.List<string>
            {
                $"startDate={start}",
                $"endDate={end}",
                $"page={model.page}",
                $"pageSize={model.pageSize}"
            };

            void AddIfNotEmpty(string key, string? value)
            {
                if (!string.IsNullOrEmpty(value))
                    queryParts.Add($"{key}={Uri.EscapeDataString(value)}");
            }

            AddIfNotEmpty("lane_Nr", model.lane_Nr);
            AddIfNotEmpty("TollOperatorID", model.TollOperatorID);
            AddIfNotEmpty("Shift", model.Shift);
            AddIfNotEmpty("PaymentMethod", model.PaymentMethod);

            var url = $"{baseUrl}?{string.Join("&", queryParts)}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return new TransactionInputModel();

            var json = await response.Content.ReadAsStringAsync();
            var pageTransactions = JsonConvert.DeserializeObject<PageTransactionModel>(json);

            if (pageTransactions == null) return new TransactionInputModel();

            return new TransactionInputModel
            {
                page = pageTransactions.page,
                pageSize = pageTransactions.pageSize,
                totalCount = pageTransactions.totalCount,
                totalPages = pageTransactions.totalPages,
                items = pageTransactions.items,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                lane_Nr = model.lane_Nr,
                TollOperatorID = model.TollOperatorID,
                Shift = model.Shift,
                PaymentMethod = model.PaymentMethod
            };
        }
    }
}
