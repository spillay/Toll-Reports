using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class VirtualPlaza
{
    public byte VirtualPlazaId { get; set; }

    public string VirtualPlazaCode { get; set; } = null!;

    public string VirtualPlazaName { get; set; } = null!;

    public byte? TollPlazaId { get; set; }

    public virtual ICollection<Lane> Lanes { get; set; } = new List<Lane>();

    public virtual TollPlaza? TollPlaza { get; set; }
}
