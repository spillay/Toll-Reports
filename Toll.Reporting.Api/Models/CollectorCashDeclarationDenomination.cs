using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class CollectorCashDeclarationDenomination
{
    public long CollectorCashDeclarationId { get; set; }

    public int DenominationId { get; set; }

    public int NumberOf { get; set; }

    public double Amount { get; set; }

    public virtual CollectorCashDeclaration CollectorCashDeclaration { get; set; } = null!;

    public virtual Denomination Denomination { get; set; } = null!;
}
