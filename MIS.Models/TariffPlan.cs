using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class TariffPlan
    {
        public TariffPlan()
        {
            TariffPlanDetails = new HashSet<TariffPlanDetail>();
            Transactions = new HashSet<Transaction>();
        }

        public int TariffPlanId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public byte CurrencyId { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual ICollection<TariffPlanDetail> TariffPlanDetails { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
