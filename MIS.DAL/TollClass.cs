using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIS.Models;

namespace MIS.DAL
{
    public class TollClass
    {
        public List<Models.TollClass> GetAll()
        {
            List<Models.TollClass> tollClasses = new List<Models.TollClass>();

            using (var db = new Models.MISDBContext())
            {
                tollClasses = db.TollClasses.OrderBy(o => o.DisplayOrder).ToList();
            }

            return tollClasses;
        }
		
        public Models.TollClass Save(Models.TollClass TollClass)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                dBContext.TollClasses.Add(TollClass);
                dBContext.SaveChanges();
            }

            return TollClass;
        }

        public void Update(Models.TollClass TollClass)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                dBContext.TollClasses.Attach(TollClass);
                dBContext.Entry(TollClass).State = EntityState.Modified;
                dBContext.SaveChanges();
            }
        }
		
    }
}
