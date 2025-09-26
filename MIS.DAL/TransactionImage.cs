using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIS.Models;

namespace MIS.DAL
{
    public class TransactionImage
    {
        public void Save(List<Models.TransactionImage> TransactionImage)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                dBContext.TransactionImages.AddRange(TransactionImage);
                dBContext.SaveChanges();
            }
        }

        public void Save(Models.TransactionImage TransactionImages)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                dBContext.TransactionImages.Add(TransactionImages);
                dBContext.SaveChanges();
            }
        }

        public List<Models.TransactionImage> GetBytTransactionId(byte LaneId, long TransactionNumber)
        {
            List<Models.TransactionImage> images = new List<Models.TransactionImage>();

            using (var db = new Models.MISDBContext())
            {
                images = db.TransactionImages.Where(x => x.LaneId == LaneId && x.TransactionNumber == TransactionNumber).OrderBy(o => o.LaneTransactionImageId).ToList();
            }

            return images;
        }
    }
}
