using MIS.Web.Models;
using MIS.Web.Models.Transaction;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MIS.Web.Services
{
    public interface IReportService
    {
        Task<TransactionInputModel> GetTransactionDetailsAsync(
            TransactionInputModel model
        );

    }
}
