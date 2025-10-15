namespace MIS.Web.Models.VarientPerfomance
{
    public class PageVarientPerfomanceModel
    {
        public List<VarientPerfomanceModel>? items { get; set; } 
        public int totalCount { get; set; } 
        public int page { get; set; } 
        public int pageSize { get; set; } 
        public int totalPages { get; set; } 
        
    }
}
