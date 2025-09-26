using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIS.Models;

namespace MIS.DAL
{
    public class TollPlaza
    {
        public List<Models.TollPlaza> GetAll()
        {
            List<Models.TollPlaza> tollPlazas = new List<Models.TollPlaza>();

            using (var db = new Models.MISDBContext())
            {
                tollPlazas = db.TollPlazas.OrderBy(o => o.PlazaName).Include(x => x.ControlCentre).ToList();
            }

            return tollPlazas;
        }

        public Models.TollPlaza Save(Models.TollPlaza TollPlaza)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                dBContext.TollPlazas.Add(TollPlaza);
                dBContext.SaveChanges();
            }

            return TollPlaza;
        }

        public void Update(Models.TollPlaza TollPlaza)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                dBContext.TollPlazas.Attach(TollPlaza);
                dBContext.Entry(TollPlaza).State = EntityState.Modified;
                dBContext.SaveChanges();
            }
        }
    }
}
