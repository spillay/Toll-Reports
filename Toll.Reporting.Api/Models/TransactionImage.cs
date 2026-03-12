using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class TransactionImage
{
    public long TransactionImageId { get; set; }

    public long LaneTransactionImageId { get; set; }

    public byte LaneId { get; set; }

    public long TransactionNumber { get; set; }

    public string SnapShot { get; set; } = null!;

    public virtual Transaction Transaction { get; set; } = null!;
}
