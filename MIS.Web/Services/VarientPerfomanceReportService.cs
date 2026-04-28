using MIS.Web.Models.VarientPerfomance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MIS.Web.Services
{
    public class VarientPerfomanceReportService : IVarientPerfomanceReportService
    {
        private readonly IApiClientService _apiClient;

        public VarientPerfomanceReportService(IApiClientService apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        public async Task<PageVarientPerfomanceModel> GetVarientPerfomanceDetailsAsync(
            int pageNumber,
            int pageSize,
            DateTime startDate,
            DateTime endDate,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null)
        {
            try
            {
                var queryParts = new List<string>
                {
                    $"startDate={Uri.EscapeDataString(startDate.ToString("s"))}",
                    $"endDate={Uri.EscapeDataString(endDate.ToString("s"))}",
                    $"page={pageNumber}",
                    $"pageSize={pageSize}"
                };

                AddIfAny(queryParts, "operationalShift", operationalShift);
                AddIfAny(queryParts, "tollOperators", tollOperators);

                return await _apiClient.GetAsync<PageVarientPerfomanceModel>(
                    "ApiSettings:VarientPerformanceEndpoint",
                    queryParts)
                    ?? new PageVarientPerfomanceModel();
            }
            catch
            {
                return new PageVarientPerfomanceModel();
            }
        }

        public async Task<List<string>> GetAllShiftsAsync()
        {
            try
            {
                var result = await _apiClient.GetAsync<List<string>>(
                    "ApiSettings:VarientPerformanceShiftsEndpoint")
                    ?? new List<string>();

                return CleanSorted(result);
            }
            catch
            {
                return new List<string>();
            }
        }

        public async Task<List<string>> GetAllTollOperatorsAsync()
        {
            try
            {
                var result = await _apiClient.GetAsync<List<string>>(
                    "ApiSettings:VarientPerformanceOperatorsEndpoint")
                    ?? new List<string>();

                return CleanSorted(result);
            }
            catch
            {
                return new List<string>();
            }
        }

        private static void AddIfAny(List<string> queryParts, string key, List<string>? list)
        {
            if (list == null || !list.Any())
                return;

            foreach (var value in list.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                queryParts.Add($"{key}={Uri.EscapeDataString(value.Trim())}");
            }
        }

        private static List<string> CleanSorted(IEnumerable<string> values)
        {
            return values
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();
        }
    }
}
