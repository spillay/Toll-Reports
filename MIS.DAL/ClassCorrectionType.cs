using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using MIS.Models;
using TollReportingSystem.Data;

namespace MIS.DAL
{
    public class ClassCorrectionType
    {
        public List<Models.ClassCorrectionType> GetAll()
        {
            List<Models.ClassCorrectionType> classCorrectionTypes = new List<Models.ClassCorrectionType>();

            using (var db = new ApplicationDbContext())
            {
                classCorrectionTypes = db.ClassCorrectionTypes
                    .OrderBy(o => o.Description)
                    .ToList();
            }

            return classCorrectionTypes;
        }
    }
}