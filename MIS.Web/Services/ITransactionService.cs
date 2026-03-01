using MIS.Web.Models;
using MIS.Web.Models.Transaction;
using System.Threading.Tasks;

namespace MIS.Web.Services
{
    public interface ITransactionService
    {
        // Paginated table data
        Task<PageTransactionModel> GetTransactionDetailsAsync(TransactionInputModel model);

        // Filter dropdowns
        Task<TransactionInputModel> GetTransactionFilterOptionsAsync(TransactionInputModel model);

        // Full unpaginated dataset for export
        Task<PageTransactionModel> GetTransactionExportAsync(TransactionInputModel model);
    }
}
