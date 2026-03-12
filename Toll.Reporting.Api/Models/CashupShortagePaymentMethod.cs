using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class CashupShortagePaymentMethod
{
    public byte CashupShortagePaymentMethodId { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<CollectorCashupShortagePayment> CollectorCashupShortagePayments { get; set; } = new List<CollectorCashupShortagePayment>();
}
