using Microsoft.AspNetCore.Mvc;
using MIS.Models;

namespace MIS.Web.Models
{
    public class VarientPerfomanceInputModel
    {
        public DateTime StartDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        //Test


        public DateTime EndDate { get; set; }

        public List<VarientPerfomance.VarientPerfomanceModel> VarientPerfomances { get; set; } = new();
        public int TotalCount { get; set; }
        // Bound filters
        //[BindProperty(SupportsGet = true)] public int PageNumber { get; set; } 
        //[BindProperty(SupportsGet = true)] public int PageSize { get; set; }
        //[BindProperty(SupportsGet = true)] public DateTime StartDate { get; set; } = DateTime.Now.AddDays(-90);
        //[BindProperty(SupportsGet = true)] public DateTime EndDate { get; set; } = DateTime.Now;
        //[BindProperty(SupportsGet = true)] public string? lane_Nr { get; set; }
        //[BindProperty(SupportsGet = true)] public string? TollOperatorID { get; set; }
        //[BindProperty(SupportsGet = true)] public string? Shift { get; set; }
        //[BindProperty(SupportsGet = true)] public string? PaymentMethod { get; set; }
        //[BindProperty(SupportsGet = true)] public string? SortOrder { get; set; }
    }
}
