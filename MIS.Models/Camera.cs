using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class Camera
    {
        public Camera()
        {
            LaneCameras = new HashSet<LaneCamera>();
        }

        public byte CameraId { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public virtual ICollection<LaneCamera> LaneCameras { get; set; }
    }
}
