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
    public class SystemUserRole
    {
        public Models.SystemUserRole Save(Models.SystemUserRole SystemUserRole)
        {
            using (ApplicationDbContext dBContext = new ApplicationDbContext())
            {
                dBContext.SystemUserRoles.Add(SystemUserRole);
                dBContext.SaveChanges();
            }

            return SystemUserRole;
        }

        public void Update(Models.SystemUserRole SystemUserRole)
        {
            using (ApplicationDbContext dBContext = new ApplicationDbContext())
            {
                dBContext.SystemUserRoles.Attach(SystemUserRole);
                dBContext.Entry(SystemUserRole).State = EntityState.Modified;
                dBContext.SaveChanges();
            }
        }
    }
}
