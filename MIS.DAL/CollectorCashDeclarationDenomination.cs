using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIS.Models;

namespace MIS.DAL
{
    public class CollectorCashDeclarationDenomination
    {
        public List<Models.CollectorCashDeclarationDenomination> Get(long CollectorCashDeclarationId)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                return dBContext.CollectorCashDeclarationDenominations
                    .Include(z => z.Denomination)
                    .Where(x => x.CollectorCashDeclarationId == CollectorCashDeclarationId)
                    .OrderBy(x => x.Denomination.DisplayOrder)
                    .ToList();
            }
        }
    }
}
