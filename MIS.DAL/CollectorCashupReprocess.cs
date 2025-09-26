using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MIS.Models;

namespace MIS.DAL
{
    public class CollectorCashupReprocess
    {
        public Models.CollectorCashupReprocess Create(Models.CollectorCashupReprocess CollectorCashupReprocess)
        {
            using (var dbContext = new Models.MISDBContext())
            {
                dbContext.CollectorCashupReprocesses.Add(CollectorCashupReprocess);
                dbContext.SaveChanges();
            }

            return CollectorCashupReprocess;
        }

        public Models.CollectorCashupReprocess Update(Models.CollectorCashupReprocess CollectorCashupReprocess)
        {
            using (var dbContext = new Models.MISDBContext())
            {
                dbContext.CollectorCashupReprocesses.Attach(CollectorCashupReprocess);
                dbContext.Entry(CollectorCashupReprocess).State = EntityState.Modified;
                dbContext.SaveChanges();
            }

            return CollectorCashupReprocess;
        }
    }
}
