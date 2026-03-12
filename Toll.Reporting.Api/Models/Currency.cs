using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class Currency
{
    public byte CurrencyId { get; set; }

    public string Description { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string Symbol { get; set; } = null!;

    public virtual ICollection<Denomination> Denominations { get; set; } = new List<Denomination>();

    public virtual ICollection<TariffPlan> TariffPlans { get; set; } = new List<TariffPlan>();

    public virtual ICollection<Transaction1> Transaction1s { get; set; } = new List<Transaction1>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
