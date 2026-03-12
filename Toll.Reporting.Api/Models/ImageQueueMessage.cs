using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class ImageQueueMessage
{
    public byte Id { get; set; }

    public string? LaneCode { get; set; }

    public int? CameraId { get; set; }

    public int? TransactionNumber { get; set; }

    public DateTime? TakeAt { get; set; }

    public byte[]? Image { get; set; }

    public string? RawJson { get; set; }

    public DateTime? CreatedAt { get; set; }
}
