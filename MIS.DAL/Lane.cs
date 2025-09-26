using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIS.Models;

namespace MIS.DAL
{
    public class Lane
    {
        public List<Models.Lane> GetAll()
        {
            List<Models.Lane> lanes = new List<Models.Lane>();

            using (var db = new Models.MISDBContext())
            {
                lanes = db.Lanes.OrderBy(o => o.LaneCode).ThenBy(o => o.LaneName).Include(x => x.VirtualPlaza)
                    .ToList();
            }

            return lanes;
        }

        public List<Models.Lane> GetByVirtualPlaza(byte VirtualPlazaId)
        {
            List<Models.Lane> lanes = new List<Models.Lane>();

            using (var db = new Models.MISDBContext())
            {
                lanes = db.Lanes.Where(x => x.VirtualPlazaId == VirtualPlazaId).OrderBy(o => o.LaneName).ToList();
            }

            return lanes;
        }

        public Models.Lane Save(Models.Lane Lane)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                dBContext.Lanes.Add(Lane);
                dBContext.SaveChanges();
            }

            return Lane;
        }

        public void Update(Models.Lane Lane)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                dBContext.Lanes.Attach(Lane);
                dBContext.Entry(Lane).State = EntityState.Modified;
                dBContext.SaveChanges();
            }
        }
    }
}
