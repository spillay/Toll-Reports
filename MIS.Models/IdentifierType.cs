using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class IdentifierType
    {
        public IdentifierType()
        {
            RegisteredUserIdentifiers = new HashSet<RegisteredUserIdentifier>();
        }

        public byte IdentifierTypeId { get; set; }
        public string Description { get; set; }

        public virtual ICollection<RegisteredUserIdentifier> RegisteredUserIdentifiers { get; set; }
    }
}
