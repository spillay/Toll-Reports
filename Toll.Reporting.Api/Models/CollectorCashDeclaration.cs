using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class CollectorCashDeclaration
{
    public long CollectorCashDeclarationId { get; set; }

    public DateOnly ShiftDate { get; set; }

    public byte ShiftId { get; set; }

    public long SystemUserId { get; set; }

    public double TotalDeclared { get; set; }

    public DateTime DeclaredAt { get; set; }

    public long VerifiedById { get; set; }

    public long? AllocatedToCollectorCashupId { get; set; }

    public virtual CollectorCashup? AllocatedToCollectorCashup { get; set; }

    public virtual ICollection<CollectorCashDeclarationDenomination> CollectorCashDeclarationDenominations { get; set; } = new List<CollectorCashDeclarationDenomination>();

    public virtual Shift Shift { get; set; } = null!;

    public virtual SystemUser SystemUser { get; set; } = null!;

    public virtual SystemUser VerifiedBy { get; set; } = null!;
}
