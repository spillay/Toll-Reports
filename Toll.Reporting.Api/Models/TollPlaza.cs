using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class TollPlaza
{
    public byte TollPlazaId { get; set; }

    public string PlazaName { get; set; } = null!;

    public string PlazaCode { get; set; } = null!;

    public byte ControlCentreId { get; set; }

    public virtual ControlCentre ControlCentre { get; set; } = null!;

    public virtual ICollection<VirtualPlaza> VirtualPlazas { get; set; } = new List<VirtualPlaza>();
}
