using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class Currency
{
    public byte CurrencyId { get; set; }

    public string Description { get; set; }

    public string Code { get; set; }

    public string Symbol { get; set; }

    public virtual ICollection<Denomination> Denominations { get; set; } = new List<Denomination>();

    public virtual ICollection<TariffPlan> TariffPlans { get; set; } = new List<TariffPlan>();

    public virtual ICollection<Transaction1> Transaction1s { get; set; } = new List<Transaction1>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
