using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIS.Models;

namespace MIS.DAL
{
    public class Denomination
    {
        public List<Models.Denomination> GetAll()
        {
            List<Models.Denomination> denomination = new List<Models.Denomination>();

            using (var db = new Models.MISDBContext())
            {
                denomination = db.Denominations.OrderBy(o => o.DisplayOrder).ToList();
            }

            return denomination;
        }
    }
}
