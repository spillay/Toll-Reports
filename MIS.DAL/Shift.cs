using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIS.Models;

namespace MIS.DAL
{
    public class Shift
    {
        public List<Models.Shift> GetAll()
        {
            List<Models.Shift> shifts = new List<Models.Shift>();

            using (var db = new Models.MISDBContext())
            {
                shifts = db.Shifts.OrderBy(o => o.ShiftId).ThenBy(o => o.StartTimeHour).ThenBy(o => o.EndTimeHour).ThenBy(o => o.Description).ToList();
            }

            return shifts;
        }

        public Models.Shift Save(Models.Shift Shift)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                dBContext.Shifts.Add(Shift);
                dBContext.SaveChanges();
            }

            return Shift;
        }

        public void Update(Models.Shift Shift)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                dBContext.Shifts.Attach(Shift);
                dBContext.Entry(Shift).State = EntityState.Modified;
                dBContext.SaveChanges();
            }
        }
    }
}
