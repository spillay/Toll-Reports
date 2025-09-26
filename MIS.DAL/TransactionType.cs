using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIS.Models;

namespace MIS.DAL
{
    public class TransactionType
    {
        public List<Models.TransactionType> GetAll()
        {
            List<Models.TransactionType> transactionTypes = new List<Models.TransactionType>();

            using (var db = new Models.MISDBContext())
            {
                transactionTypes = db.TransactionTypes.OrderBy(o => o.Description).ToList();
            }

            return transactionTypes;
        }
    }
}
