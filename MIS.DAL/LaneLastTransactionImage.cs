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
    public class LaneLastTransactionImage
    {
        public List<Models.LaneLastTransactionImage> GetAll()
        {
            List<Models.LaneLastTransactionImage> laneLastTransactionImage = new List<Models.LaneLastTransactionImage>();

            using (var db = new ApplicationDbContext())
            {
                laneLastTransactionImage = db.LaneLastTransactionImages.ToList();
            }

            return laneLastTransactionImage;
        }
    }
}
