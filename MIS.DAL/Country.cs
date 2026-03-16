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
    public class Country
    {
        public List<Models.Country> GetAll()
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Countries.OrderBy(x => x.CountryId).ToList();
            }
        }


        public Models.Country Save(Models.Country Country)
        {
            using (ApplicationDbContext dBContext = new ApplicationDbContext())
            {
                dBContext.Countries.Add(Country);
                dBContext.SaveChanges();
            }

            return Country;
        }

        public void Update(Models.Country Country)
        {
            using (ApplicationDbContext dBContext = new ApplicationDbContext())
            {
                dBContext.Countries.Attach(Country);
                dBContext.Entry(Country).State = EntityState.Modified;
                dBContext.SaveChanges();
            }
        }
    }
}

