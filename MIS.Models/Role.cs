using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class Role
    {
        public Role()
        {
            SystemUserRoles = new HashSet<SystemUserRole>();
        }

        public short RoleId { get; set; }
        public string Description { get; set; }

        public virtual ICollection<SystemUserRole> SystemUserRoles { get; set; }
    }
}
