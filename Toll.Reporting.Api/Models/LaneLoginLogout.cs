using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class LaneLoginLogout
{
    public long LaneLoginLogoutId { get; set; }

    public byte LaneId { get; set; }

    public long SystemUserId { get; set; }

    public byte ShiftId { get; set; }

    public DateOnly ShiftDate { get; set; }

    public DateTime LoginAt { get; set; }

    public DateTime? LogOutAt { get; set; }
}
