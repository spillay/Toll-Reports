using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class Camera
{
    public byte CameraId { get; set; }

    public string Description { get; set; } = null!;

    public string ImageUrl { get; set; } = null!;

    public string? Username { get; set; }

    public string? Password { get; set; }

    public virtual ICollection<LaneCamera> LaneCameras { get; set; } = new List<LaneCamera>();
}
