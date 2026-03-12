using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class LaneDisplayMessage
{
    public int LaneDisplayMessageId { get; set; }

    public string English { get; set; } = null!;

    public string ToDisplay { get; set; } = null!;
}
