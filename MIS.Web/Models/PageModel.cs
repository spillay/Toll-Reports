namespace MIS.Web.Models
{
    public class PageModel
    {
        public int totalCount { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
        public int totalPages { get; set; }
        public int pageCount() { return (int)Math.Ceiling((double)totalCount / pageSize); }

    }
}
