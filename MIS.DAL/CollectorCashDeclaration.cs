using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using MIS.Models;
using TollReportingSystem.Data;

namespace MIS.DAL
{
    public class CollectorCashDeclaration
    {
        public Models.CollectorCashDeclaration Save(Models.CollectorCashDeclaration collectorCashDeclaration)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                dbContext.CollectorCashDeclarations.Add(collectorCashDeclaration);
                dbContext.SaveChanges();
            }

            return collectorCashDeclaration;
        }

        public Models.CollectorCashDeclaration Get(long collectorCashDeclarationId)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                return dbContext.CollectorCashDeclarations
                    .Include(z => z.SystemUser)
                    .Include(z => z.VerifiedBy)
                    .Include(z => z.Shift)
                    .Where(x => x.CollectorCashDeclarationId == collectorCashDeclarationId)
                    .FirstOrDefault();
            }
        }

        public List<Models.CollectorCashDeclaration> GetList(DateTime shiftDate, byte shiftId)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                return dbContext.CollectorCashDeclarations
                    .Include(z => z.SystemUser)
                    .Include(z => z.VerifiedBy)
                    .Include(z => z.Shift)
                    .Where(x => x.ShiftDate == shiftDate && x.ShiftId == shiftId)
                    .ToList();
            }
        }

        public List<Models.CollectorCashDeclaration> GetForCashup(Models.CollectorCashup collectorCashup)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                return dbContext.CollectorCashDeclarations
                    .Include(z => z.SystemUser)
                    .Include(z => z.VerifiedBy)
                    .Include(z => z.Shift)
                    .Where(x =>
                        x.ShiftDate == collectorCashup.ShiftDate &&
                        x.ShiftId == collectorCashup.ShiftId &&
                        x.SystemUserId == collectorCashup.SystemUserId &&
                        (!x.AllocatedToCollectorCashupId.HasValue || x.AllocatedToCollectorCashupId == collectorCashup.CollectorCashupId))
                    .ToList();
            }
        }

        public List<Models.CollectorCashDeclaration> Update(List<Models.CollectorCashDeclaration> collectorCashDeclarations)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                dbContext.CollectorCashDeclarations.AttachRange(collectorCashDeclarations);

                foreach (var item in collectorCashDeclarations)
                {
                    dbContext.Entry(item).State = EntityState.Modified;
                }

                dbContext.SaveChanges();
            }

            return collectorCashDeclarations;
        }

        public List<Models.CollectorCashDeclaration> GetCashDeclarationsVerifiedBy(DateTime shiftDate, byte shiftId, long verifiedById)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                return dbContext.CollectorCashDeclarations
                    .Include(z => z.SystemUser)
                    .Include(z => z.VerifiedBy)
                    .Include(z => z.Shift)
                    .Where(x => x.ShiftDate == shiftDate && x.ShiftId == shiftId && x.VerifiedById == verifiedById)
                    .ToList();
            }
        }
    }
}