using Newtonsoft.Json;
using System.Collections.Generic;

namespace MIS.Web.Models.Discrepancy
{
    public class PageDiscrepancyModel : PageModel
    {
        [JsonProperty("items")]
        public List<DiscrepancyModel> Items { get; set; } = new();

        [JsonProperty("fullItems")]
        public List<DiscrepancyModel>? FullItems { get; set; }

        [JsonProperty("totalCount")]
        public int totalCount { get; set; }

        [JsonProperty("page")]
        public int page { get; set; }

        [JsonProperty("pageSize")]
        public int pageSize { get; set; }

        [JsonProperty("totalPages")]
        public int totalPages { get; set; }

        public DiscrepancyInputModel Filters { get; set; } = new();
        public List<DiscrepancyModel> ExportItems { get; set; }

        public int TotalRecords => totalCount;
    }
}
