using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class RegisteredUserPlazaTransactionSetting
    {
        public long RegisteredUserId { get; set; }
        public byte TollPlazaId { get; set; }
        public byte TransactionTypeId { get; set; }
        public byte DiscountTypeId { get; set; }

        public virtual RegisteredUser RegisteredUser { get; set; }
        public virtual TollPlaza TollPlaza { get; set; }
    }
}
