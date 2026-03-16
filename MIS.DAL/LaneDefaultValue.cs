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
    public class LaneDefaultValue
    {
        public List<Models.LaneDefaultValue> GetAll()
        {
            List<Models.LaneDefaultValue> laneDefaultValues = new List<Models.LaneDefaultValue>();

            using (var db = new ApplicationDbContext())
            {
                laneDefaultValues = db.LaneDefaultValues.OrderBy(o => o.LaneDefaultValueId).ToList();
            }

            return laneDefaultValues;
        }

        public Models.LaneDefaultValue Save(Models.LaneDefaultValue LaneDefaultValue)
        {
            using (ApplicationDbContext dBContext = new ApplicationDbContext())
            {
                dBContext.LaneDefaultValues.Add(LaneDefaultValue);
                dBContext.SaveChanges();
            }

            return LaneDefaultValue;
        }

        public void Update(Models.LaneDefaultValue LaneDefaultValue)
        {
            using (ApplicationDbContext dBContext = new ApplicationDbContext())
            {
                dBContext.LaneDefaultValues.Attach(LaneDefaultValue);
                dBContext.Entry(LaneDefaultValue).State = EntityState.Modified;
                dBContext.SaveChanges();
            }
        }
    }
}
