using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class ControlCentre
    {
        public ControlCentre()
        {
            TollPlazas = new HashSet<TollPlaza>();
        }

        public byte ControlCentreId { get; set; }
        public string ControlCentreName { get; set; }
        public string ControlCentreCode { get; set; }

        public virtual ICollection<TollPlaza> TollPlazas { get; set; }
    }
}
