using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class LaneDefaultValue
{
    public int LaneDefaultValueId { get; set; }

    public string DefaultValueDescriptions { get; set; }

    public int? Ivalue { get; set; }

    public string Svalue { get; set; }

    public DateOnly? Dvalue { get; set; }

    public string Cvalue { get; set; }

    public bool? Bvalue { get; set; }
}
