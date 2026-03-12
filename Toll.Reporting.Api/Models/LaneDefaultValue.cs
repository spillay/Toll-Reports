using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class LaneDefaultValue
{
    public int LaneDefaultValueId { get; set; }

    public string DefaultValueDescriptions { get; set; } = null!;

    public int? Ivalue { get; set; }

    public string? Svalue { get; set; }

    public DateOnly? Dvalue { get; set; }

    public string? Cvalue { get; set; }

    public bool? Bvalue { get; set; }
}
