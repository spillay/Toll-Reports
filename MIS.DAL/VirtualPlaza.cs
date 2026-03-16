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
    public class VirtualPlaza
    {
        public List<Models.VirtualPlaza> GetAll()
        {
            List<Models.VirtualPlaza> virtualPlazas = new List<Models.VirtualPlaza>();

            using (var db = new ApplicationDbContext())
            {
                virtualPlazas = db.VirtualPlazas.OrderBy(o => o.VirtualPlazaName).Include(x => x.TollPlaza).ToList();
            }

            return virtualPlazas;
        }

        public List<Models.VirtualPlaza> GetByTollPlaza(byte TollPlazaId)
        {
            List<Models.VirtualPlaza> virtualPlazas = new List<Models.VirtualPlaza>();

            using (var db = new ApplicationDbContext())
            {
                virtualPlazas = db.VirtualPlazas.Where(x=> x.TollPlazaId == TollPlazaId).OrderBy(o => o.VirtualPlazaName).ToList();
            }

            return virtualPlazas;
        }

        public Models.VirtualPlaza Save(Models.VirtualPlaza VirtualPlaza)
        {
            using (ApplicationDbContext dBContext = new ApplicationDbContext())
            {
                dBContext.VirtualPlazas.Add(VirtualPlaza);
                dBContext.SaveChanges();
            }

            return VirtualPlaza;
        }

        public void Update(Models.VirtualPlaza VirtualPlaza)
        {
            using (ApplicationDbContext dBContext = new ApplicationDbContext())
            {
                dBContext.VirtualPlazas.Attach(VirtualPlaza);
                dBContext.Entry(VirtualPlaza).State = EntityState.Modified;
                dBContext.SaveChanges();
            }
        }
    }
}
