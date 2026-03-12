using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class Role
{
    public short RoleId { get; set; }

    public string Description { get; set; }

    public virtual ICollection<SystemUserRole> SystemUserRoles { get; set; } = new List<SystemUserRole>();
}
