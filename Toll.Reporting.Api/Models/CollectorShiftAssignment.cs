using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class CollectorShiftAssignment
{
    public long SystemUserId { get; set; }

    public DateOnly ShiftDate { get; set; }

    public byte ShiftId { get; set; }

    public byte ShiftStatusId { get; set; }

    public virtual Shift Shift { get; set; } = null!;

    public virtual SystemUser SystemUser { get; set; } = null!;
}
