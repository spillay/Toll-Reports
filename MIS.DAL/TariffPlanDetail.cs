using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS.DAL
{
    public class TariffPlanDetail
    {
        public List<Models.TariffPlanDetail> GetAll()
        {
            List<Models.TariffPlanDetail> tariffPlanDetails = new List<Models.TariffPlanDetail>();

            using (var db = new Models.MISDBContext())
            {
                tariffPlanDetails = db.TariffPlanDetails.ToList();
            }

            return tariffPlanDetails;
        }
    }
}
