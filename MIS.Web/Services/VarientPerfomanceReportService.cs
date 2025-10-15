using MIS.Web.Models.VarientPerfomance;
using Newtonsoft.Json;


namespace MIS.Web.Services
{
    public class VarientPerfomanceReportService : IVarientPerfomanceReportService
    {
        private readonly HttpClient _httpClient;

        public VarientPerfomanceReportService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PageVarientPerfomanceModel> GetVarientPerfomanceDetailsAsync(
            int pageNumber,
            int pageSize,
            DateTime startDate,
            DateTime endDate,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null
            )
        {
            string formattedStart = startDate.ToString("s"); // yyyy-MM-ddTHH:mm:ss
            string formattedEnd = endDate.ToString("s");
            string formattedPageNumber = pageNumber.ToString();
            string formattedPageSize = pageSize.ToString();

             var url = $"http://localhost:5000/api/VarientPerformance/details?startDate={Uri.EscapeDataString(formattedStart)}&endDate={Uri.EscapeDataString(formattedEnd)}&page={Uri.EscapeDataString(formattedPageNumber)}&pageSize={Uri.EscapeDataString(formattedPageSize)}";

            if (operationalShift != null && operationalShift.Any())
                url += $"&operationalShift={Uri.EscapeDataString(string.Join(",", operationalShift))}";

            if (tollOperators != null && tollOperators.Any())
                url += $"&tollOperators={Uri.EscapeDataString(string.Join(",", tollOperators))}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return new PageVarientPerfomanceModel();
            }

            var json = await response.Content.ReadAsStringAsync();

            var varientPerfomances = JsonConvert.DeserializeObject<PageVarientPerfomanceModel>(json);

            return varientPerfomances ?? new PageVarientPerfomanceModel();
        }
    }
}
