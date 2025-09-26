using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class VirtualPlaza
    {
        public VirtualPlaza()
        {
            Lanes = new HashSet<Lane>();
        }

        public byte VirtualPlazaId { get; set; }
        public string VirtualPlazaCode { get; set; }
        public string VirtualPlazaName { get; set; }
        public byte? TollPlazaId { get; set; }

        public virtual TollPlaza TollPlaza { get; set; }
        public virtual ICollection<Lane> Lanes { get; set; }
    }
}
