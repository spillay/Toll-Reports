using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class SystemUserRole
{
    public long SystemUserId { get; set; }

    public short RoleId { get; set; }

    public bool IsActive { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual SystemUser SystemUser { get; set; } = null!;
}
