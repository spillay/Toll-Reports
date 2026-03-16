using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using MIS.Models;
using TollReportingSystem.Data;

namespace MIS.DAL
{
    public class CollectorCashDeclarationDenomination
    {
        public List<Models.CollectorCashDeclarationDenomination> Get(long collectorCashDeclarationId)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                return dbContext.CollectorCashDeclarationDenominations
                    .Include(z => z.Denomination)
                    .Where(x => x.CollectorCashDeclarationId == collectorCashDeclarationId)
                    .OrderBy(x => x.Denomination.DisplayOrder)
                    .ToList();
            }
        }
    }
}