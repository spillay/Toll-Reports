using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class TariffPlan
{
    public int TariffPlanId { get; set; }

    public DateOnly EffectiveDate { get; set; }

    public byte CurrencyId { get; set; }

    public virtual Currency Currency { get; set; }

    public virtual ICollection<TariffPlanDetail> TariffPlanDetails { get; set; } = new List<TariffPlanDetail>();

    public virtual ICollection<Transaction1> Transaction1s { get; set; } = new List<Transaction1>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
