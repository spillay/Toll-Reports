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
    public class SupervisorCashup
    {
        public Models.SupervisorCashup Create(Models.SupervisorCashup SupervisorCashup)
        {
            using (ApplicationDbContext dBContext = new ApplicationDbContext())
            {
                dBContext.SupervisorCashups.Add(SupervisorCashup);
                dBContext.SaveChanges();
            }
            return SupervisorCashup;
        }

        public List<Models.SupervisorCashup> GetUnverified(DateTime ShiftDate, byte ShiftId)
        {
            using (ApplicationDbContext dBContext = new ApplicationDbContext())
            {
                return dBContext.SupervisorCashups
                            .Where(x => x.ShiftId == ShiftId && x.ShiftDate == ShiftDate && !x.VerifiedById.HasValue)
                            .Include(z => z.SystemUser)
                            .Include(z => z.Shift).ToList();
            }
        }

        public Models.SupervisorCashup Update(Models.SupervisorCashup SupervisorCashup)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                dbContext.SupervisorCashups.Attach(SupervisorCashup);
                dbContext.Entry(SupervisorCashup).State = EntityState.Modified;
                dbContext.SaveChanges();
            }

            return SupervisorCashup;
        }
    }
}
