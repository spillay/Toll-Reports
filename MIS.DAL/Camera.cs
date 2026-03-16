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
    public class Camera
    {
        public List<Models.Camera> GetAll()
        {
            List<Models.Camera> Cameras = new List<Models.Camera>();

            using (var db = new ApplicationDbContext())
            {
                Cameras = db.Cameras.OrderBy(o => o.CameraId).ToList();
            }

            return Cameras;
        }

        public Models.Camera Save(Models.Camera Camera)
        {
            using (ApplicationDbContext dBContext = new ApplicationDbContext())
            {
                dBContext.Cameras.Add(Camera);
                dBContext.SaveChanges();
            }

            return Camera;
        }

        public void Update(Models.Camera Camera)
        {
            using (ApplicationDbContext dBContext = new ApplicationDbContext())
            {
                dBContext.Cameras.Attach(Camera);
                dBContext.Entry(Camera).State = EntityState.Modified;
                dBContext.SaveChanges();
            }
        }
    }
}
