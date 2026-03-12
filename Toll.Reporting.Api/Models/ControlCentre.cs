using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class ControlCentre
{
    public byte ControlCentreId { get; set; }

    public string ControlCentreName { get; set; } = null!;

    public string ControlCentreCode { get; set; } = null!;

    public virtual ICollection<TollPlaza> TollPlazas { get; set; } = new List<TollPlaza>();
}
