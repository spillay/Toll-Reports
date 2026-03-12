using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class ReguserStat
{
    public string Status { get; set; } = null!;

    public int? StatusCount { get; set; }

    public double? TotalValue { get; set; }
}
