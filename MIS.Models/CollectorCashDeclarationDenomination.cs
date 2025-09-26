using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{

    public partial class CollectorCashDeclarationDenomination
    {
        public long CollectorCashDeclarationId { get; set; }
        public int DenominationId { get; set; }
        public int NumberOf { get; set; }
        public double Amount { get; set; }

        public virtual CollectorCashDeclaration CollectorCashDeclaration { get; set; }
        public virtual Denomination Denomination { get; set; }
    }
}
