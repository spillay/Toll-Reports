using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIS.Models;

namespace MIS.DAL
{
    public class SystemUserRole
    {
        public Models.SystemUserRole Save(Models.SystemUserRole SystemUserRole)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                dBContext.SystemUserRoles.Add(SystemUserRole);
                dBContext.SaveChanges();
            }

            return SystemUserRole;
        }

        public void Update(Models.SystemUserRole SystemUserRole)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                dBContext.SystemUserRoles.Attach(SystemUserRole);
                dBContext.Entry(SystemUserRole).State = EntityState.Modified;
                dBContext.SaveChanges();
            }
        }
    }
}
