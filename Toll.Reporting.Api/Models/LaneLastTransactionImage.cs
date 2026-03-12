using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class LaneLastTransactionImage
{
    public byte LaneId { get; set; }

    public long LastLaneTransactionImageId { get; set; }

    public virtual Lane Lane { get; set; } = null!;
}
