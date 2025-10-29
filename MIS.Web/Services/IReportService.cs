using MIS.Web.Models;
using MIS.Web.Models.Transaction;
using System.Threading.Tasks;

namespace MIS.Web.Services
{
    public interface IReportService
    {
        Task<PageTransactionModel> GetTransactionDetailsAsync(TransactionInputModel model);
        Task<FilterOptionsModel> GetTransactionFilterOptionsAsync(TransactionInputModel model);
    }
}
