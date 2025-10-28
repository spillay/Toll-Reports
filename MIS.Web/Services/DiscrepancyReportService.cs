using MIS.Web.Models.Discrepancy;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MIS.Web.Services
{

    public class DiscrepancyReportService : IDiscrepancyReportService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public DiscrepancyReportService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<PageDiscrepancyModel> GetDiscrepancyReportAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null,
            List<string>? laneNames = null,
            List<string>? paymentMethods = null,
            List<string>? takenAction = null,
            int page = 1,
            int pageSize = 50)
        {
            //var baseUrl = _configuration["ApiSettings:DiscrepancyReportApiUrl"];
            //if (string.IsNullOrEmpty(baseUrl))
            //    throw new InvalidOperationException("ApiSettings:DiscrepancyReportApiUrl not set");

            var q = new List<string>
            {
                $"startDate={Uri.EscapeDataString(startDate.ToString("yyyy-MM-dd"))}",
                $"endDate={Uri.EscapeDataString(endDate.ToString("yyyy-MM-dd"))}",
                $"page={page}",
                $"pageSize={pageSize}"
            };

            void AddList(string key, List<string>? list)
            {
                if (list?.Any() == true)
                {
                    foreach (var v in list) q.Add($"{key}={Uri.EscapeDataString(v)}");
                }
            }

            AddList("operationalShift", operationalShift);
            AddList("tollOperators", tollOperators);
            AddList("laneNames", laneNames);
            AddList("paymentMethods", paymentMethods);
            AddList("takenAction", takenAction);

            string baseUrl = _configuration["BaseApiUrl:Link"];
            string endpoint = _configuration["ApiSettings:DiscrepancyReportEndpoint"];
            string url = $"{baseUrl}{endpoint}?{string.Join("&", q)}";

            try
            {
                var res = await _httpClient.GetAsync(url);
                if (!res.IsSuccessStatusCode)
                    return CreateEmptyModel(startDate, endDate, page, pageSize);

                var body = await res.Content.ReadAsStringAsync();
                var pagedDto = JsonConvert.DeserializeObject<ApiPagedResult>(body);

                var items = (pagedDto?.Items ?? new List<ApiDiscrepancyDto>())
                    .Select(d => new DiscrepancyModel
                    {
                        lane_Nr = d.Lane_Nr.ToString(),
                        trx_Sequence_Nr = int.TryParse(d.Trx_Sequence_Nr, out var seq) ? seq : 0,
                        trx_Date = d.Trx_Date,
                        trx_Time = d.Trx_Time,
                        operational_Shift = d.Operational_Shift,
                        toll_Operator_ID = d.Toll_Operator_ID,
                        lane_Name = d.Lane_Name,
                        method_of_Payment = d.Method_of_Payment,
                        toll_Collector_Class = d.Toll_Collector_Class,
                        avC_Class = d.AVC_Class,
                        final_Class = d.Final_Class,
                        tariff = d.Tariff ?? 0,
                        updated_tariff = d.Updated_Tariff ?? 0,
                        takenAction = d.TakenAction
                    })
                    .ToList();

                return new PageDiscrepancyModel
                {
                    Items = items,
                    Filters = new DiscrepancyInputModel
                    {
                        StartDate = startDate,
                        EndDate = endDate,
                        Page = pagedDto?.Page ?? page,
                        PageSize = pagedDto?.PageSize ?? pageSize
                    },
                    totalCount = pagedDto?.TotalCount ?? items.Count,
                    page = pagedDto?.Page ?? page,
                    pageSize = pagedDto?.PageSize ?? pageSize,
                    totalPages = pagedDto?.TotalPages ?? (pagedDto?.PageSize == 0 ? 0 : (int)Math.Ceiling((double)(pagedDto?.TotalCount ?? items.Count) / (pagedDto?.PageSize ?? pageSize)))
                };
            }
            catch
            {
                return CreateEmptyModel(startDate, endDate, page, pageSize);
            }
        }

        private PageDiscrepancyModel CreateEmptyModel(DateTime s, DateTime e, int page, int pageSize) =>
            new PageDiscrepancyModel
            {
                Items = new List<DiscrepancyModel>(),
                Filters = new DiscrepancyInputModel { StartDate = s, EndDate = e, Page = page, PageSize = pageSize },
                totalCount = 0,
                page = page,
                pageSize = pageSize,
                totalPages = 0
            };

        private class ApiDiscrepancyDto
        {
            public int Lane_Nr { get; set; }
            public string Trx_Sequence_Nr { get; set; } = string.Empty;
            public string Trx_Date { get; set; } = string.Empty;
            public string Trx_Time { get; set; } = string.Empty;
            public string Operational_Shift { get; set; } = string.Empty;
            public string Toll_Operator_ID { get; set; } = string.Empty;
            public string Lane_Name { get; set; } = string.Empty;
            public string Method_of_Payment { get; set; } = string.Empty;
            public string Toll_Collector_Class { get; set; } = string.Empty;
            public string AVC_Class { get; set; } = string.Empty;
            public string Final_Class { get; set; } = string.Empty;
            public decimal? Tariff { get; set; }
            public decimal? Updated_Tariff { get; set; }
            public string TakenAction { get; set; } = string.Empty;
        }

        private class ApiPagedResult
        {
            public List<ApiDiscrepancyDto> Items { get; set; } = new();
            public int TotalCount { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
            public int TotalPages { get; set; }
        }
    }
}
