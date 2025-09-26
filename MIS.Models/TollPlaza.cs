using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class TollPlaza
    {
        public TollPlaza()
        {
            RegisteredUserIdentifierTollPlazaApplicableDiscounts = new HashSet<RegisteredUserIdentifierTollPlazaApplicableDiscount>();
            RegisteredUserPlazaTransactionSettings = new HashSet<RegisteredUserPlazaTransactionSetting>();
            VirtualPlazas = new HashSet<VirtualPlaza>();
        }

        public byte TollPlazaId { get; set; }
        public string PlazaName { get; set; }
        public string PlazaCode { get; set; }
        public byte ControlCentreId { get; set; }

        public virtual ControlCentre ControlCentre { get; set; }
        public virtual ICollection<RegisteredUserIdentifierTollPlazaApplicableDiscount> RegisteredUserIdentifierTollPlazaApplicableDiscounts { get; set; }
        public virtual ICollection<RegisteredUserPlazaTransactionSetting> RegisteredUserPlazaTransactionSettings { get; set; }
        public virtual ICollection<VirtualPlaza> VirtualPlazas { get; set; }
    }
}
