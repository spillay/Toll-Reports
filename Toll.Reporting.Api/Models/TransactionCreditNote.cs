using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class TransactionCreditNote
{
    public long CreditNoteId { get; set; }

    public byte LaneId { get; set; }

    public long TransactionNumber { get; set; }

    public virtual Transaction Transaction { get; set; } = null!;
}
