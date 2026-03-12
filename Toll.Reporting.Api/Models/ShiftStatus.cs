using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class ShiftStatus
{
    public byte ShiftStatusId { get; set; }

    public string Description { get; set; } = null!;
}
