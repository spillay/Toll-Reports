using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class TransactionMissingDetail
{
    public long TransactionNumber { get; set; }

    public byte LaneId { get; set; }

    public bool Received { get; set; }

    public DateTime? ReceivedAt { get; set; }
}
