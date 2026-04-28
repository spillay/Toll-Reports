using MIS.Web.Models;
using MIS.Web.Models.TopUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MIS.Web.Services
{
    public class TopUpReportService : ITopUpReportService
    {
        private readonly IApiClientService _apiClient;

        public TopUpReportService(IApiClientService apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        public async Task<TopUpInputModel> GetTopUpFilterOptionsAsync()
        {
            try
            {
                return await _apiClient.GetAsync<TopUpInputModel>(
                    "ApiSettings:TopUpFilterOptionsEndpoint")
                    ?? new TopUpInputModel();
            }
            catch
            {
                return new TopUpInputModel();
            }
        }

        public Task<PageTopUpModel> GetTopUpAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? shifts = null,
            List<string>? operatorIds = null,
            List<string>? lanes = null,
            List<string>? paymentMethods = null,
            string? accountNumber = null,
            int page = 1,
            int pageSize = 30)
        {
            return GetTopUpInternalAsync(
                startDate,
                endDate,
                shifts,
                operatorIds,
                lanes,
                paymentMethods,
                accountNumber,
                page,
                pageSize);
        }

        public Task<PageTopUpModel> GetTopUpFullAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? shifts = null,
            List<string>? operatorIds = null,
            List<string>? lanes = null,
            List<string>? paymentMethods = null,
            string? accountNumber = null)
        {
            return GetTopUpInternalAsync(
                startDate,
                endDate,
                shifts,
                operatorIds,
                lanes,
                paymentMethods,
                accountNumber,
                1,
                1000000);
        }

        private async Task<PageTopUpModel> GetTopUpInternalAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? shifts,
            List<string>? operatorIds,
            List<string>? lanes,
            List<string>? paymentMethods,
            string? accountNumber,
            int page,
            int pageSize)
        {
            try
            {
                var query = new List<string>
                {
                    $"startDate={Uri.EscapeDataString(startDate.ToString("yyyy-MM-ddTHH:mm:ss"))}",
                    $"endDate={Uri.EscapeDataString(endDate.ToString("yyyy-MM-ddTHH:mm:ss"))}",
                    $"page={page}",
                    $"pageSize={pageSize}"
                };

                AddListParams(query, "shifts", shifts);
                AddListParams(query, "operatorIds", operatorIds);
                AddListParams(query, "lanes", lanes);
                AddListParams(query, "paymentMethods", paymentMethods);

                if (!string.IsNullOrWhiteSpace(accountNumber))
                    query.Add($"accountNumber={Uri.EscapeDataString(accountNumber.Trim())}");

                return await _apiClient.GetAsync<PageTopUpModel>(
                    "ApiSettings:TopUpEndpoint",
                    query)
                    ?? new PageTopUpModel();
            }
            catch
            {
                return new PageTopUpModel();
            }
        }

        private static void AddListParams(List<string> query, string key, List<string>? values)
        {
            if (values == null)
                return;

            foreach (var value in values.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()))
            {
                query.Add($"{key}={Uri.EscapeDataString(value)}");
            }
        }
    }
}
