using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class LaneCamera
{
    public byte LaneId { get; set; }

    public byte CameraId { get; set; }

    public bool? DefaultCamera { get; set; }

    public virtual Camera Camera { get; set; } = null!;

    public virtual Lane Lane { get; set; } = null!;
}
