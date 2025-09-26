using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class CashupShortagePaymentMethod
    {
        public CashupShortagePaymentMethod()
        {
            CollectorCashupShortagePayments = new HashSet<CollectorCashupShortagePayment>();
        }

        public byte CashupShortagePaymentMethodId { get; set; }
        public string Description { get; set; }

        public virtual ICollection<CollectorCashupShortagePayment> CollectorCashupShortagePayments { get; set; }
    }
}
