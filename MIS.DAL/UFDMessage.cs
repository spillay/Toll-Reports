using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIS.Models;

namespace MIS.DAL
{
    public class UFDMessage
    {
        public List<Models.Ufdmessage> GetAll()
        {
            List<Models.Ufdmessage> ufdMessages = new List<Models.Ufdmessage>();

            using (var db = new Models.ApplicationDbContext())
            {
                ufdMessages = db.Ufdmessages.OrderBy(o => o.UfdmessageId).ToList();
            }

            return ufdMessages;
        }

        public Models.Ufdmessage Save(Models.Ufdmessage UFDMessage)
        {
            using (Models.ApplicationDbContext dBContext = new Models.ApplicationDbContext())
            {
                dBContext.Ufdmessages.Add(UFDMessage);
                dBContext.SaveChanges();
            }

            return UFDMessage;
        }

        public void Update(Models.Ufdmessage UFDMessage)
        {
            using (Models.ApplicationDbContext dBContext = new Models.ApplicationDbContext())
            {
                dBContext.Ufdmessages.Attach(UFDMessage);
                dBContext.Entry(UFDMessage).State = EntityState.Modified;
                dBContext.SaveChanges();
            }
        }
    }
}
