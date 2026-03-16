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
    public class LaneCamera
    {
        public List<Models.LaneCamera> GetAll()
        {
            List<Models.LaneCamera> LaneCameras = new List<Models.LaneCamera>();

            using (var db = new ApplicationDbContext())
            {
                LaneCameras = db.LaneCameras.OrderBy(o => o.LaneId).ToList();
            }

            return LaneCameras;
        }

        public Models.LaneCamera Save(Models.LaneCamera LaneCamera)
        {
            using (ApplicationDbContext dBContext = new ApplicationDbContext())
            {
                dBContext.LaneCameras.Add(LaneCamera);
                dBContext.SaveChanges();
            }

            return LaneCamera;
        }

        public void Update(Models.LaneCamera LaneCamera)
        {
            using (ApplicationDbContext dBContext = new ApplicationDbContext())
            {
                dBContext.LaneCameras.Attach(LaneCamera);
                dBContext.Entry(LaneCamera).State = EntityState.Modified;
                dBContext.SaveChanges();
            }
        }
    }
}
