using System;

namespace MIS.Web.Models
{
    public class PageModel
    {
        private int _page = 1;
        private int _pageSize = 50;

        public int totalCount { get; set; }

        public int page
        {
            get => _page;
            set => _page = (value <= 0 ? 1 : value);
        }

        public int pageSize
        {
            get => _pageSize;
            set => _pageSize = (value <= 0 ? 50 : value);
        }

        public int totalPages { get; set; }

        public int pageCount()
        {
            if (pageSize <= 0)
                pageSize = 50;
            return (int)Math.Ceiling((double)totalCount / pageSize);
        }
    }
}
