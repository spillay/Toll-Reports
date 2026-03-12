using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class TransactionType
{
    public byte TransactionTypeId { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<TariffPlanDetail> TariffPlanDetails { get; set; } = new List<TariffPlanDetail>();

    public virtual ICollection<Transaction1> Transaction1s { get; set; } = new List<Transaction1>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
