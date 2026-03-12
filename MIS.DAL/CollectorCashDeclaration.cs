using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIS.Models;

namespace MIS.DAL
{
    public class CollectorCashDeclaration
    {
        public Models.CollectorCashDeclaration Save(Models.CollectorCashDeclaration CollectorCashDeclaration)
        {
            using (Models.ApplicationDbContext dBContext = new Models.ApplicationDbContext())
            {
                dBContext.CollectorCashDeclarations.Add(CollectorCashDeclaration);
                dBContext.SaveChanges();
            }
            return CollectorCashDeclaration;
        }

        public Models.CollectorCashDeclaration Get(long CollectorCashDeclarationId)
        {
            using (Models.ApplicationDbContext dBContext = new Models.ApplicationDbContext())
            {
                return dBContext.CollectorCashDeclarations
                    .Include(z => z.SystemUser)
                    .Include(z => z.VerifiedBy)
                    .Include(z => z.Shift)
                    .Where(x => x.CollectorCashDeclarationId == CollectorCashDeclarationId).FirstOrDefault();
            }
        }

        public List<Models.CollectorCashDeclaration> GetList(DateTime ShiftDate, byte ShiftId)
        {
            using (Models.ApplicationDbContext dBContext = new Models.ApplicationDbContext())
            {
                return dBContext.CollectorCashDeclarations
                    .Include(z => z.SystemUser)
                    .Include(z => z.VerifiedBy)
                    .Include(z => z.Shift)
                    .Where(x => x.ShiftDate == ShiftDate && x.ShiftId == ShiftId).ToList();
            }
        }

        public List<Models.CollectorCashDeclaration> GetForCashup(Models.CollectorCashup CollectorCashup)
        {
            using (Models.ApplicationDbContext dBContext = new Models.ApplicationDbContext())
            {
                return dBContext.CollectorCashDeclarations
                    .Include(z => z.SystemUser)
                    .Include(z => z.VerifiedBy)
                    .Include(z => z.Shift)
                    .Where(x => x.ShiftDate == CollectorCashup.ShiftDate && x.ShiftId == CollectorCashup.ShiftId && x.SystemUserId == CollectorCashup.SystemUserId &&
                        (!x.AllocatedToCollectorCashupId.HasValue | x.AllocatedToCollectorCashupId == CollectorCashup.CollectorCashupId)).ToList();
            }
        }

        public List<Models.CollectorCashDeclaration> Update(List<Models.CollectorCashDeclaration> CollectorCashDeclarations)
        {
            using (Models.ApplicationDbContext dBContext = new Models.ApplicationDbContext())
            {
                dBContext.CollectorCashDeclarations.AttachRange(CollectorCashDeclarations);
                foreach(var item in CollectorCashDeclarations)
                    dBContext.Entry(item).State = EntityState.Modified;
                dBContext.SaveChanges();
            }
            return CollectorCashDeclarations;
        }

        public List<Models.CollectorCashDeclaration> GetCashDeclarationsVerifiedBy(DateTime ShiftDate, byte ShiftId, long VerifiedById)
        {
            using (Models.ApplicationDbContext dBContext = new Models.ApplicationDbContext())
            {
                return dBContext.CollectorCashDeclarations
                    .Include(z => z.SystemUser)
                    .Include(z => z.VerifiedBy)
                    .Include(z => z.Shift)
                    .Where(x => x.ShiftDate == ShiftDate && x.ShiftId == ShiftId && x.VerifiedById == VerifiedById).ToList();
            }
        }
    }
}
