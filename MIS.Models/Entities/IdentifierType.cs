using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class IdentifierType
{
    public byte IdentifierTypeId { get; set; }

    public string Description { get; set; }

    public virtual ICollection<RegisteredUserIdentifier> RegisteredUserIdentifiers { get; set; } = new List<RegisteredUserIdentifier>();
}
