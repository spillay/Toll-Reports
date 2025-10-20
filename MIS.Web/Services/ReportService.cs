using MIS.Web.Models;
using MIS.Web.Models.Transaction;
using Newtonsoft.Json;
using System;
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

        public async Task<TransactionInputModel> GetTransactionDetailsAsync(TransactionInputModel model)
        {
            string start = model.StartDate.ToString("s");
            string end = model.EndDate.ToString("s");

            var url = $"http://localhost:5000/api/Transaction/details?startDate={Uri.EscapeDataString(start)}&endDate={Uri.EscapeDataString(end)}&page={model.page}&pageSize={model.pageSize}";

            if (!string.IsNullOrEmpty(model.lane_Nr)) url += $"&lane_Nr={Uri.EscapeDataString(model.lane_Nr)}";
            if (!string.IsNullOrEmpty(model.TollOperatorID)) url += $"&TollOperatorID={Uri.EscapeDataString(model.TollOperatorID)}";
            if (!string.IsNullOrEmpty(model.Shift)) url += $"&Shift={Uri.EscapeDataString(model.Shift)}";
            if (!string.IsNullOrEmpty(model.PaymentMethod)) url += $"&PaymentMethod={Uri.EscapeDataString(model.PaymentMethod)}";

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
