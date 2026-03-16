using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TollReportingSystem.Data;

namespace MIS.DAL
{
    public class CollectorCashupShortagePayment
    {
        public Models.CollectorCashupShortagePayment Create(Models.CollectorCashupShortagePayment CollectorCashupShortagePayment)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                dbContext.CollectorCashupShortagePayments.Add(CollectorCashupShortagePayment);
                dbContext.SaveChanges();
            }

            return CollectorCashupShortagePayment;
        }

        public Models.CollectorCashupShortagePayment Update(Models.CollectorCashupShortagePayment CollectorCashupShortagePayment)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                dbContext.CollectorCashupShortagePayments.Attach(CollectorCashupShortagePayment);
                dbContext.Entry(CollectorCashupShortagePayment).State = EntityState.Modified;
                dbContext.SaveChanges();
            }

            return CollectorCashupShortagePayment;
        }


        public List<Models.CollectorCashupShortagePayment> GetByCollectorCashupId(long CollectorCashupId)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                return dbContext.CollectorCashupShortagePayments.Where(z => z.CollectorCashupId == CollectorCashupId).ToList();
            }
        }

        public Models.CollectorCashupShortagePayment Get(long CollectorCashupShortagePaymentId)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                return dbContext.CollectorCashupShortagePayments
                        .Include(x => x.CollectorCashup)
                            .ThenInclude(x => x.SystemUser)
                        .Include(x => x.CollectorCashup)
                            .ThenInclude(x => x.Shift)
                        .Include(x => x.ReceivedBy)
                        .Include(x => x.CashupShortagePaymentMethod)
                        .Where(z => z.CollectorCashupShortagePaymentId == CollectorCashupShortagePaymentId).FirstOrDefault();
            }
        }
    }
}
