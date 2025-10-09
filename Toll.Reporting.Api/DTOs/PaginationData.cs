namespace Toll.Reporting.Api.DTOs
{
    public class PaginationData
    {

        public PaginationData(int totalCount, int currentPage , int itemPerPage) 
        { 
          TotalCount = totalCount;
          CurrentPage = currentPage;
          TotalPages = (int)Math.Ceiling(totalCount /(double)itemPerPage);
        }

        public int CurrentPage { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalCount;
    }
}

