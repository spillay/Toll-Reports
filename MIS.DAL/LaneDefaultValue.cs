using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIS.Models;

namespace MIS.DAL
{
    public class LaneDefaultValue
    {
        public List<Models.LaneDefaultValue> GetAll()
        {
            List<Models.LaneDefaultValue> laneDefaultValues = new List<Models.LaneDefaultValue>();

            using (var db = new Models.MISDBContext())
            {
                laneDefaultValues = db.LaneDefaultValues.OrderBy(o => o.LaneDefaultValueId).ToList();
            }

            return laneDefaultValues;
        }

        public Models.LaneDefaultValue Save(Models.LaneDefaultValue LaneDefaultValue)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                dBContext.LaneDefaultValues.Add(LaneDefaultValue);
                dBContext.SaveChanges();
            }

            return LaneDefaultValue;
        }

        public void Update(Models.LaneDefaultValue LaneDefaultValue)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                dBContext.LaneDefaultValues.Attach(LaneDefaultValue);
                dBContext.Entry(LaneDefaultValue).State = EntityState.Modified;
                dBContext.SaveChanges();
            }
        }
    }
}
