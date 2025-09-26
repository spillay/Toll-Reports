using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS.DAL
{
    public class TariffPlan
    {
        public List<Models.TariffPlan> GetAll()
        {
            List<Models.TariffPlan> tariffPlans = new List<Models.TariffPlan>();

            using (var db = new Models.MISDBContext())
            {
                tariffPlans = db.TariffPlans.ToList();
            }

            return tariffPlans;
        }

        public Models.TariffPlan Get(int TariffPlanId)
        {
            Models.TariffPlan tariffPlans = new();

            using (var db = new Models.MISDBContext())
            {
                tariffPlans = db.TariffPlans.Where(x => x.TariffPlanId == TariffPlanId)
                    .Include(x => x.TariffPlanDetails)
                    .FirstOrDefault();
            }

            return tariffPlans;
        }
    }
}
