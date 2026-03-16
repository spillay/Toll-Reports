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
    public class ExemptType
    {
        public List<Models.ExemptType> GetAll()
        {
            using (var db = new ApplicationDbContext())
            {
                return db.ExemptTypes.OrderBy(x => x.DisplayOrder).ToList();
            }
        }


        public Models.ExemptType Save(Models.ExemptType ExemptType)
        {
            using (ApplicationDbContext dBContext = new ApplicationDbContext())
            {
                dBContext.ExemptTypes.Add(ExemptType);
                dBContext.SaveChanges();
            }

            return ExemptType;
        }

        public void Update(Models.ExemptType ExemptType)
        {
            using (ApplicationDbContext dBContext = new ApplicationDbContext())
            {
                dBContext.ExemptTypes.Attach(ExemptType);
                dBContext.Entry(ExemptType).State = EntityState.Modified;
                dBContext.SaveChanges();
            }
        }
    }
}

