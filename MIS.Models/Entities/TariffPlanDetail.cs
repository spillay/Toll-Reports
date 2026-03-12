using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class TariffPlanDetail
{
    public int TariffPlanId { get; set; }

    public byte TollClassId { get; set; }

    public byte TransactionTypeId { get; set; }

    public double AmountInclusive { get; set; }

    public double Vat { get; set; }

    public double DiscountAmount { get; set; }

    public double AmountExclusive { get; set; }

    public double VatRate { get; set; }

    public virtual TariffPlan TariffPlan { get; set; }

    public virtual TollClass TollClass { get; set; }

    public virtual TransactionType TransactionType { get; set; }
}
