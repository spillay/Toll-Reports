using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIS.Models;

namespace MIS.DAL
{
    public class LaneDisplayMessage
    {
        public List<Models.LaneDisplayMessage> GetAll()
        {
            List<Models.LaneDisplayMessage> laneDefaultValues = new List<Models.LaneDisplayMessage>();

            using (var db = new Models.ApplicationDbContext())
            {
                laneDefaultValues = db.LaneDisplayMessages.OrderBy(o => o.LaneDisplayMessageId).ToList();
            }

            return laneDefaultValues;
        }

        public Models.LaneDisplayMessage Save(Models.LaneDisplayMessage LaneDisplayMessage)
        {
            using (Models.ApplicationDbContext dBContext = new Models.ApplicationDbContext())
            {
                dBContext.LaneDisplayMessages.Add(LaneDisplayMessage);
                dBContext.SaveChanges();
            }

            return LaneDisplayMessage;
        }

        public void Update(Models.LaneDisplayMessage LaneDisplayMessage)
        {
            using (Models.ApplicationDbContext dBContext = new Models.ApplicationDbContext())
            {
                dBContext.LaneDisplayMessages.Attach(LaneDisplayMessage);
                dBContext.Entry(LaneDisplayMessage).State = EntityState.Modified;
                dBContext.SaveChanges();
            }
        }
    }
}
