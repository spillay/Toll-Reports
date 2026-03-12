using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class TollPlaza
{
    public byte TollPlazaId { get; set; }

    public string PlazaName { get; set; }

    public string PlazaCode { get; set; }

    public byte ControlCentreId { get; set; }

    public virtual ControlCentre ControlCentre { get; set; }

    public virtual ICollection<VirtualPlaza> VirtualPlazas { get; set; } = new List<VirtualPlaza>();
}
