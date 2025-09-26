using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIS.Models;

namespace MIS.DAL
{
    public class ControlCentre
    {
        public List<Models.ControlCentre> GetAll()
        {
            List<Models.ControlCentre> controlCentres = new List<Models.ControlCentre>();

            using (var db = new Models.MISDBContext())
            {
                controlCentres = db.ControlCentres.OrderBy( o => o.ControlCentreName).ToList();
            }

            return controlCentres;
        }

        public Models.ControlCentre Save(Models.ControlCentre ControlCentre)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                dBContext.ControlCentres.Add(ControlCentre);
                dBContext.SaveChanges();
            }

            return ControlCentre;
        }

        public void Update(Models.ControlCentre ControlCentre)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                dBContext.ControlCentres.Attach(ControlCentre);
                dBContext.Entry(ControlCentre).State = EntityState.Modified;
                dBContext.SaveChanges();
            }
        }
    }
}
