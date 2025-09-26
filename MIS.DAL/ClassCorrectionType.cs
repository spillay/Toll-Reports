using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIS.Models;


namespace MIS.DAL
{
    public class ClassCorrectionType
    {
        public List<Models.ClassCorrectionType> GetAll()
        {
            List<Models.ClassCorrectionType> classCorrectionTypes = new List<Models.ClassCorrectionType>();

            using (var db = new Models.MISDBContext())
            {
                classCorrectionTypes = db.ClassCorrectionTypes.OrderBy(o => o.Description).ToList();
            }

            return classCorrectionTypes;
        }
    }
}
