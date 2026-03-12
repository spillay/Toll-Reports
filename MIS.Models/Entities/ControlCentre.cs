using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class ControlCentre
{
    public byte ControlCentreId { get; set; }

    public string ControlCentreName { get; set; }

    public string ControlCentreCode { get; set; }

    public virtual ICollection<TollPlaza> TollPlazas { get; set; } = new List<TollPlaza>();
}
