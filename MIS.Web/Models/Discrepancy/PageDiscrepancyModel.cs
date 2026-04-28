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

        public DiscrepancyInputModel Filters { get; set; } = new();
        public List<DiscrepancyModel> ExportItems { get; set; } = new();

        public int TotalRecords => totalCount;
    }
}
