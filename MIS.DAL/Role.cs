using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIS.Models;

namespace MIS.DAL
{
    public class Role
    {
        public List<Models.Role> GetAll()
        {
            List<Models.Role> roles = new List<Models.Role>();

            using (var db = new Models.MISDBContext())
            {
                roles = db.Roles.OrderBy(o => o.Description).ToList();
            }

            return roles;
        }
    }
}
