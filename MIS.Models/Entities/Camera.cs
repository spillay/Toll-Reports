using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class Camera
{
    public byte CameraId { get; set; }

    public string Description { get; set; }

    public string ImageUrl { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public virtual ICollection<LaneCamera> LaneCameras { get; set; } = new List<LaneCamera>();
}
