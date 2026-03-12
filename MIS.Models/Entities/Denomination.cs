using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class Denomination
{
    public int DenominationId { get; set; }

    public byte CurrencyId { get; set; }

    public string Description { get; set; }

    public double Mulitplier { get; set; }

    public int DisplayOrder { get; set; }

    public virtual ICollection<CollectorCashDeclarationDenomination> CollectorCashDeclarationDenominations { get; set; } = new List<CollectorCashDeclarationDenomination>();

    public virtual Currency Currency { get; set; }
}
