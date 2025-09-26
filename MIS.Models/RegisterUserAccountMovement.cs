using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class RegisterUserAccountMovement
    {
        public long RegisterUserAccountMovementId { get; set; }
        public long RegisterUserId { get; set; }
        public long? RegisteredUserTopUpId { get; set; }
        public long? RegisteredUserLaneTransactionId { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public string Description { get; set; }
        public double OpeningBalance { get; set; }
        public double? Debit { get; set; }
        public double? Credit { get; set; }
        public double ClosingBalance { get; set; }

        public virtual RegisteredUser RegisterUser { get; set; }
        public virtual RegisteredUserTopUp RegisteredUserTopUp { get; set; }
    }
}
