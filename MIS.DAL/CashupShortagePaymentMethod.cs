using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIS.Models;

namespace MIS.DAL
{
    public class CashupShortagePaymentMethod
    {
        public Models.CashupShortagePaymentMethod Create(Models.CashupShortagePaymentMethod CashupShortagePaymentMethod)
        {
            using (var dbContext = new Models.MISDBContext())
            {
                dbContext.CashupShortagePaymentMethods.Add(CashupShortagePaymentMethod);
                dbContext.SaveChanges();
            }

            return CashupShortagePaymentMethod;
        }
    }
}
