using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class RegisteredUserIdentifier
    {
        public RegisteredUserIdentifier()
        {
            RegisteredUserIdentifierTollPlazaApplicableDiscounts = new HashSet<RegisteredUserIdentifierTollPlazaApplicableDiscount>();
        }

        public long RegisteredUserIdentifierId { get; set; }
        public long RegisteredUserId { get; set; }
        public byte IdentifierTypeId { get; set; }
        public byte RegisterdTollClassId { get; set; }
        public string RegisteredIdentifier { get; set; }
        public int VehicleTypeId { get; set; }
        public int? VehicleColour { get; set; }
        public string NumberPlateDetails { get; set; }
        public double CurrentDiscountPercentage { get; set; }
        public double CurrentDiscountAmount { get; set; }
        public byte[] RowVersion { get; set; }
        public bool LowBalanceWarning { get; set; }
        public double LowBalanceValue { get; set; }
        public byte LowBalanceTripCount { get; set; }
        public bool? IsActive { get; set; }
        public bool HotListed { get; set; }
        public DateTime ActivationDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        public virtual IdentifierType IdentifierType { get; set; }
        public virtual RegisteredUser RegisteredUser { get; set; }
        public virtual VehicleType VehicleType { get; set; }
        public virtual ICollection<RegisteredUserIdentifierTollPlazaApplicableDiscount> RegisteredUserIdentifierTollPlazaApplicableDiscounts { get; set; }
    }
}
