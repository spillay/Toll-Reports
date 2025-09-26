using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class RegisteredUserContact
    {
        public long RegisteredUserId { get; set; }
        public byte ContactTypeId { get; set; }
        public string ContactDetails { get; set; }

        public virtual RegisteredUser RegisteredUser { get; set; }
    }
}
