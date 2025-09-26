using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MIS.Models;

namespace MIS.DAL
{
    public class CollectorCashup
    {
        public Models.CollectorCashup Create(Models.CollectorCashup CollectorCashup)
        {
            using (var dbContext = new Models.MISDBContext())
            {
                dbContext.CollectorCashups.Add(CollectorCashup);
                dbContext.SaveChanges();
            }

            return CollectorCashup;
        }

        public Models.CollectorCashup Update(Models.CollectorCashup CollectorCashup)
        {
            using (var dbContext = new Models.MISDBContext())
            {
                dbContext.CollectorCashups.Attach(CollectorCashup);
                dbContext.Entry(CollectorCashup).State = EntityState.Modified;
                dbContext.SaveChanges();
            }

            return CollectorCashup;
        }

        public Models.CollectorCashup Get(long CollectorCashupId)
        {
            using (var dbContext = new Models.MISDBContext())
            {
                return dbContext.CollectorCashups.Where(x => x.CollectorCashupId == CollectorCashupId).Include(z => z.SystemUser).Include(z => z.Shift).FirstOrDefault();
            }
        }

        public List<Models.CollectorCashup> Get(DateTime ShiftDate, byte ShiftId)
        {
            using (var dbContext = new Models.MISDBContext())
            {
                return dbContext.CollectorCashups.Where(x => x.ShiftDate == ShiftDate && x.ShiftId == ShiftId).Include(z => z.SystemUser).Include(z => z.Shift).ToList();
            }
        }


        public List<Models.CollectorCashup> Get(DateTime StartShiftDate, DateTime EndShiftDate, List<Models.SystemUser> Collectors)
        {
            using (var dbContext = new Models.MISDBContext())
            {
                return dbContext.CollectorCashups.
                        Include(z => z.SystemUser).
                        Include(z => z.Shift).
                        Where(x => x.ShiftDate >= StartShiftDate && x.ShiftDate <= EndShiftDate &&
                                (Collectors == null || Collectors.Contains(x.SystemUser))
                                ).ToList();
            }
        }
    }
}
