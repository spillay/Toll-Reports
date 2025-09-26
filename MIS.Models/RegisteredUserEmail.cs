using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class RegisteredUserEmail
    {
        public long RegisteredUserId { get; set; }
        public byte EmailTypeId { get; set; }
        public string EmailAddress { get; set; }

        public virtual RegisteredUser RegisteredUser { get; set; }
    }
}
