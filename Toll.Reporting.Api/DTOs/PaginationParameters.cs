namespace Toll.Reporting.Api.DTOs
{
    public class PaginationParameters
    {
        private int _maxItemsPerPage = 20;
        private int itemPerpage;
        public int Page { get; set; } = 1;
        public int ItemPerpage 
        { get => itemPerpage;
            set => itemPerpage = value > _maxItemsPerPage ? _maxItemsPerPage : value; }
    }
}
