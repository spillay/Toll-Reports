using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class ReguserStat
{
    public string Status { get; set; }

    public int? StatusCount { get; set; }

    public double? TotalValue { get; set; }
}
