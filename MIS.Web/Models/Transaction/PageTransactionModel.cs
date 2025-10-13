namespace MIS.Web.Models.Transaction
{
    public class PageTransactionModel
    {
        public List<TransactionModel> items { get; set; } 
        public int totalCount { get; set; } 
        public int page { get; set; } 
        public int pageSize { get; set; } 
        public int totalPages { get; set; } 
        
    }
}
