using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class RegisteredUserTopUp
    {
        public RegisteredUserTopUp()
        {
            RegisterUserAccountMovements = new HashSet<RegisterUserAccountMovement>();
        }

        public long RegisteredUserTopUpId { get; set; }
        public long RegisterUserId { get; set; }
        public long SystemUserId { get; set; }
        public DateTime RechargedOn { get; set; }
        public string Description { get; set; }
        public byte TransactionTypeId { get; set; }
        public byte PaymentIdentifierId { get; set; }
        public byte PaymentMethodId { get; set; }
        public byte RechargePointId { get; set; }
        public double Amount { get; set; }

        public virtual PaymentIdentifier PaymentIdentifier { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }
        public virtual RechargePoint RechargePoint { get; set; }
        public virtual RegisteredUser RegisterUser { get; set; }
        public virtual ICollection<RegisterUserAccountMovement> RegisterUserAccountMovements { get; set; }
    }
}
