using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIS.Models;

namespace MIS.DAL
{
    public class SystemUser
    {
        public Models.SystemUser GetUserByUsername(string UserName)
        {
            Models.SystemUser systemUser = new Models.SystemUser();
            using (var db = new Models.MISDBContext())
            {
                systemUser = db.SystemUsers.Include(x => x.SystemUserRoles).Where(z => z.Username == UserName).FirstOrDefault();
            }

            return systemUser;
        }

        public List<Models.SystemUser> GetUsers(string FirstName, string LastName, string UserName)
        {
            List<Models.SystemUser> systemUser = new List<Models.SystemUser>();
            using (var db = new Models.MISDBContext())
            {
                systemUser = db.SystemUsers.Include(x => x.SystemUserRoles).
                    Include(x => x.SystemUserRoles).
                        ThenInclude(x => x.Role).
                    Where(z => z.Username.Contains(UserName.Length == 0?z.Username : UserName) &&
                            z.FirstName.Contains(FirstName.Length == 0 ? z.FirstName : FirstName) &&
                            z.LastName.Contains(LastName.Length == 0 ? z.LastName : LastName)
                        ).
                        OrderBy(z => z.FirstName).
                            ThenBy(z => z.LastName).
                            ThenBy(z => z.Username).
                        ToList();
            }

            return systemUser;
        }

        public List<Models.SystemUser> GetUsers(short RoleId)
        {
            List<Models.SystemUser> systemUser = new List<Models.SystemUser>();
            using (var db = new Models.MISDBContext())
            {
                systemUser = db.SystemUsers.
                    Include(c => c.SystemUserRoles.Where(o => o.RoleId == RoleId && o.IsActive)).
                    Where(x => x.IsActive && !x.IsLocked).
                        OrderBy(z => z.FirstName).
                            ThenBy(z => z.LastName).
                            ThenBy(z => z.Username).
                        ToList();
            }
            
            return systemUser;
        }

        public Models.SystemUser Save(Models.SystemUser SystemUser)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                dBContext.SystemUsers.Add(SystemUser);
                dBContext.SaveChanges();
            }

            return SystemUser;
        }

        public void Update(Models.SystemUser SystemUser)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                dBContext.SystemUsers.Attach(SystemUser);
                dBContext.Entry(SystemUser).State = EntityState.Modified;
                dBContext.SaveChanges();
            }
        }
    }
}
