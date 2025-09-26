using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class RegisteredUserIdentifierTollPlazaApplicableDiscount
    {
        public long RegisteredUserIdentifierId { get; set; }
        public byte TollPlazaId { get; set; }
        public double CurrentDiscountPercentage { get; set; }
        public double CurrentDiscountAmount { get; set; }

        public virtual RegisteredUserIdentifier RegisteredUserIdentifier { get; set; }
        public virtual TollPlaza TollPlaza { get; set; }
    }
}
