using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS.DAL
{
    public class CollectorCashupCashSurplusAllocatedToDiscrepancy
    {
        public Models.CollectorCashupCashSurplusAllocatedToDiscrepancy Create(Models.CollectorCashupCashSurplusAllocatedToDiscrepancy CollectorCashupCashSurplusAllocatedToDiscrepancy)
        {
            using (var dbContext = new Models.MISDBContext())
            {
                dbContext.CollectorCashupCashSurplusAllocatedToDiscrepancies.Add(CollectorCashupCashSurplusAllocatedToDiscrepancy);
                dbContext.SaveChanges();
            }

            return CollectorCashupCashSurplusAllocatedToDiscrepancy;
        }
    }
}
