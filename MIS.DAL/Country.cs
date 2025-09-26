using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIS.Models;

namespace MIS.DAL
{
    public class Country
    {
        public List<Models.Country> GetAll()
        {
            using (var db = new Models.MISDBContext())
            {
                return db.Countries.OrderBy(x => x.CountryId).ToList();
            }
        }


        public Models.Country Save(Models.Country Country)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                dBContext.Countries.Add(Country);
                dBContext.SaveChanges();
            }

            return Country;
        }

        public void Update(Models.Country Country)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                dBContext.Countries.Attach(Country);
                dBContext.Entry(Country).State = EntityState.Modified;
                dBContext.SaveChanges();
            }
        }
    }
}

