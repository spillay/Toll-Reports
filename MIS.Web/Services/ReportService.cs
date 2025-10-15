using MIS.Web.Models;
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

        public async Task<TransactionInputModel> GetTransactionDetailsAsync(TransactionInputModel model)      
           
        {
            string formattedStart = model.StartDate.ToString("s"); // yyyy-MM-ddTHH:mm:ss
            string formattedEnd = model.EndDate.ToString("s");
            string formattedPageNumber = model.page.ToString();
            string formattedPageSize = model.pageSize.ToString();

             var url = $"http://localhost:5000/api/Transaction/details?startDate={Uri.EscapeDataString(formattedStart)}&endDate={Uri.EscapeDataString(formattedEnd)}&page={Uri.EscapeDataString(formattedPageNumber)}&pageSize={Uri.EscapeDataString(formattedPageSize)}";

            //if (operationalShift != null && operationalShift.Any())
            //    url += $"&operationalShift={Uri.EscapeDataString(string.Join(",", operationalShift))}";

            //if (tollOperators != null && tollOperators.Any())
            //    url += $"&tollOperators={Uri.EscapeDataString(string.Join(",", tollOperators))}";

            //if (laneNames != null && laneNames.Any())
            //    url += $"&laneNames={Uri.EscapeDataString(string.Join(",", laneNames))}";

            //if (paymentMethods != null && paymentMethods.Any())
            //    url += $"&paymentMethods={Uri.EscapeDataString(string.Join(",", paymentMethods))}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return new TransactionInputModel();
            }

            var json = await response.Content.ReadAsStringAsync();

            var pageTransactions = JsonConvert.DeserializeObject<PageTransactionModel>(json);
            if (pageTransactions != null)
            {
                var newModel = new TransactionInputModel
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
                return newModel;
            }
            return new TransactionInputModel();
        }

        //public Task<TransactionInputModel> GetTransactionDetailsAsync(TransactionInputModel model)
        //{
        //    return TransactionDetailsAsync(model.StartDate)
        //}
    }
}
