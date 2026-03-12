using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class Role
{
    public short RoleId { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<SystemUserRole> SystemUserRoles { get; set; } = new List<SystemUserRole>();
}
