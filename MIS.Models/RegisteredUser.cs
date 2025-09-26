using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class RegisteredUser
    {
        public RegisteredUser()
        {
            RegisterUserAccountMovements = new HashSet<RegisterUserAccountMovement>();
            RegisteredUserAddresses = new HashSet<RegisteredUserAddress>();
            RegisteredUserContacts = new HashSet<RegisteredUserContact>();
            RegisteredUserEmails = new HashSet<RegisteredUserEmail>();
            RegisteredUserIdentifiers = new HashSet<RegisteredUserIdentifier>();
            RegisteredUserPlazaTransactionSettings = new HashSet<RegisteredUserPlazaTransactionSetting>();
            RegisteredUserTopUps = new HashSet<RegisteredUserTopUp>();
            Transactions = new HashSet<Transaction>();
        }

        public long RegisteredUserId { get; set; }
        public int EntityId { get; set; }
        public byte EntityTypeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdNumber { get; set; }
        public string CompanyName { get; set; }
        public string RegistrationNumber { get; set; }
        public bool? IsPrepaid { get; set; }
        public byte DiscountTypeId { get; set; }
        public byte TransactionTypeId { get; set; }
        public double Balance { get; set; }
        public bool BalanceVisibilityLane { get; set; }
        public bool BalanceVisibilityReceipt { get; set; }
        public bool BalanceVisibilityUfd { get; set; }
        public bool? IsActive { get; set; }
        public bool HotListed { get; set; }
        public string Reference { get; set; }
        public DateTime ActivationDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime BalanceChangedOn { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<RegisterUserAccountMovement> RegisterUserAccountMovements { get; set; }
        public virtual ICollection<RegisteredUserAddress> RegisteredUserAddresses { get; set; }
        public virtual ICollection<RegisteredUserContact> RegisteredUserContacts { get; set; }
        public virtual ICollection<RegisteredUserEmail> RegisteredUserEmails { get; set; }
        public virtual ICollection<RegisteredUserIdentifier> RegisteredUserIdentifiers { get; set; }
        public virtual ICollection<RegisteredUserPlazaTransactionSetting> RegisteredUserPlazaTransactionSettings { get; set; }
        public virtual ICollection<RegisteredUserTopUp> RegisteredUserTopUps { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
