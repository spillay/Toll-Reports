using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class VirtualPlaza
{
    public byte VirtualPlazaId { get; set; }

    public string VirtualPlazaCode { get; set; }

    public string VirtualPlazaName { get; set; }

    public byte? TollPlazaId { get; set; }

    public virtual ICollection<Lane> Lanes { get; set; } = new List<Lane>();

    public virtual TollPlaza TollPlaza { get; set; }
}
