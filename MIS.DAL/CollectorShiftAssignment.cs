using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIS.Models;

namespace MIS.DAL
{
    public class CollectorShiftAssignment
    {
        public Models.CollectorShiftAssignment GetBy(long SystemUserId, DateTime Shiftdate, byte ShiftId)
        {
            Models.CollectorShiftAssignment collectorShiftAssignment = new Models.CollectorShiftAssignment();

            using (var db = new Models.MISDBContext())
            {
                collectorShiftAssignment = db.CollectorShiftAssignments.Where(x => x.SystemUserId == SystemUserId && x.ShiftDate == Shiftdate && x.ShiftId == ShiftId).FirstOrDefault();
            }

            return collectorShiftAssignment;
        }

        public List<Models.CollectorShiftAssignment> GetBy(DateTime Shiftdate, byte ShiftId)
        {
            List<Models.CollectorShiftAssignment> collectorShiftAssignment = new List<Models.CollectorShiftAssignment>();

            using (var db = new Models.MISDBContext())
            {
                collectorShiftAssignment = db.CollectorShiftAssignments.Where(x => x.ShiftDate == Shiftdate && x.ShiftId == ShiftId)
                    .Include(x => x.SystemUser)
                    .ToList();
            }

            return collectorShiftAssignment;
        }

        public List<Models.SystemUser> GetUnAssignedBy(DateTime Shiftdate, byte ShiftId, byte RoleId)
        {
            List<Models.SystemUser> systemUsers = new List<Models.SystemUser>();

            using (var db = new Models.MISDBContext())
            {

                systemUsers = db.SystemUsers.Include(x => x.SystemUserRoles).Where(x => x.IsActive && 
                    x.ActivationDate <= DateTime.Now && x.SystemUserRoles.Any(f => f.RoleId == RoleId)).ToList();

                List<Models.CollectorShiftAssignment> collectorShiftAssignment = db.CollectorShiftAssignments.
                    Where(x => x.ShiftDate == Shiftdate && x.ShiftId == ShiftId)
                    .ToList();

                return systemUsers.Where(p => collectorShiftAssignment.All(p2 => p2.SystemUserId != p.SystemUserId)).ToList();
            }
        }

        public void Save(Models.CollectorShiftAssignment CollectorShiftAssignment)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                dBContext.CollectorShiftAssignments.Add(CollectorShiftAssignment);
                dBContext.SaveChanges();
            }
        }

        public void Delete(Models.CollectorShiftAssignment CollectorShiftAssignment)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                dBContext.CollectorShiftAssignments.Attach(CollectorShiftAssignment);
                dBContext.CollectorShiftAssignments.Remove(CollectorShiftAssignment);
                dBContext.SaveChanges();
            }
        }

        public Models.CollectorShiftAssignment CloseShift(long SystemUserId, DateTime Shiftdate, byte ShiftId)
        {
            Models.CollectorShiftAssignment collectorShiftAssignment = new Models.CollectorShiftAssignment();

            using (var db = new Models.MISDBContext())
            {
                collectorShiftAssignment = db.CollectorShiftAssignments.Where(x => x.SystemUserId == SystemUserId && x.ShiftDate == Shiftdate && x.ShiftId == ShiftId).First();
                collectorShiftAssignment.ShiftStatusId = (byte)Common.Enums.ShiftStatus.Closed;
                db.SaveChanges();
            }

            return collectorShiftAssignment;
        }
    }
}
