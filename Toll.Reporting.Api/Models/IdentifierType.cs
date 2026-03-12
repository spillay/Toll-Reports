using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class IdentifierType
{
    public byte IdentifierTypeId { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<RegisteredUserIdentifier> RegisteredUserIdentifiers { get; set; } = new List<RegisteredUserIdentifier>();
}
