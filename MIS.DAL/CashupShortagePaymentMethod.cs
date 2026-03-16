using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIS.Models;
using TollReportingSystem.Data;

namespace MIS.DAL
{
    public class CashupShortagePaymentMethod
    {
        public Models.CashupShortagePaymentMethod Create(Models.CashupShortagePaymentMethod cashupShortagePaymentMethod)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                dbContext.CashupShortagePaymentMethods.Add(cashupShortagePaymentMethod);
                dbContext.SaveChanges();
            }

            return cashupShortagePaymentMethod;
        }
    }
}