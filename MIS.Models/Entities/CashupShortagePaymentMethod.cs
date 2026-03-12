using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class CashupShortagePaymentMethod
{
    public byte CashupShortagePaymentMethodId { get; set; }

    public string Description { get; set; }

    public virtual ICollection<CollectorCashupShortagePayment> CollectorCashupShortagePayments { get; set; } = new List<CollectorCashupShortagePayment>();
}
