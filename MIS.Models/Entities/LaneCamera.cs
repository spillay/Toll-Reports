using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class LaneCamera
{
    public byte LaneId { get; set; }

    public byte CameraId { get; set; }

    public bool? DefaultCamera { get; set; }

    public virtual Camera Camera { get; set; }

    public virtual Lane Lane { get; set; }
}
