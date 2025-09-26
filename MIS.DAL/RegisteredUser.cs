using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIS.Models;

namespace MIS.DAL
{
    public class RegisteredUser
    {



        public List<Models.RegisteredUser> GetAll()
        {
            List<Models.RegisteredUser> RegisteredUsers = new List<Models.RegisteredUser>();

            using (var db = new Models.MISDBContext())
            {
                RegisteredUsers = db.RegisteredUsers.OrderBy(o => o.RegistrationNumber).ToList();
            }

            return RegisteredUsers;
        }


        public List<Models.RegisteredUser> GetFilter(string query)
        {
            List<Models.RegisteredUser> registeredUsers = new List<Models.RegisteredUser>();

            using (var db = new Models.MISDBContext())
            {
                registeredUsers = db.RegisteredUsers
                                .Include(r => r.RegisteredUserIdentifiers)
                                //.Include(r => r.DiscountType)
                                //.Include(r => r.AspNetUser)
                                //.Include(r => r.DiscountType)
                                //.Include(r => r.Entity)
                                //.Include(r => r.EntityType)
                                //.Include(r => r.TransactionType)
                                .Where(u => u.FirstName.Contains(query) ||
                                            u.LastName.Contains(query) ||
                                            u.RegisteredUserIdentifiers.Any(i =>
                                               i.NumberPlateDetails.Contains(query) ||
                                               i.RegisteredIdentifier.Contains(query)))
                                .ToList();
            }
            return registeredUsers;
        }
    }
}
