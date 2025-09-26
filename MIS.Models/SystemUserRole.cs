using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class SystemUserRole
    {
        public long SystemUserId { get; set; }
        public short RoleId { get; set; }
        public bool IsActive { get; set; }

        public virtual Role Role { get; set; }
        public virtual SystemUser SystemUser { get; set; }
    }
}
