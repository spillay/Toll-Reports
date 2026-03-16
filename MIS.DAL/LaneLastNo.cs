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
    public class LaneLastNo
    {
        public List<Models.LaneLastNo> GetAll()
        {
            List<Models.LaneLastNo> laneLastNos = new List<Models.LaneLastNo>();

            using (var db = new ApplicationDbContext())
            {
                laneLastNos = db.LaneLastNos.ToList();
            }

            return laneLastNos;
        }
    }
}
