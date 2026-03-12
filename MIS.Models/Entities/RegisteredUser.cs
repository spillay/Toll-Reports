using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class RegisteredUser
{
    public long RegisterUserId { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string CompanyName { get; set; }

    public bool IsPrepaid { get; set; }

    public byte DiscountTypeId { get; set; }

    public double Balance { get; set; }

    public bool BalanceVisibilityLane { get; set; }

    public bool BalanceVisibilityReceipt { get; set; }

    public bool BalanceVisibilityUfd { get; set; }

    public bool LowBalanceWarning { get; set; }

    public double LowBalanceValue { get; set; }

    public byte LowBalanceTripCount { get; set; }

    public bool IsActive { get; set; }

    public bool HotListed { get; set; }

    public DateOnly ActivationDate { get; set; }

    public DateOnly ExpiryDate { get; set; }

    public DateTime BalanceChangedOn { get; set; }

    public string Status { get; set; }

    public string AccNr { get; set; }

    public string PrimaryEmail { get; set; }

    public string SecondaryEmail { get; set; }

    public string PrimaryContact { get; set; }

    public string SecondaryContact { get; set; }

    public string Address { get; set; }

    public byte[] RowVersion { get; set; }

    public long SystemUserId { get; set; }

    public virtual ICollection<RegisterUserAccountMovement> RegisterUserAccountMovements { get; set; } = new List<RegisterUserAccountMovement>();

    public virtual ICollection<RegisteredUserIdentifier> RegisteredUserIdentifiers { get; set; } = new List<RegisteredUserIdentifier>();

    public virtual ICollection<RegisteredUserTopUp> RegisteredUserTopUps { get; set; } = new List<RegisteredUserTopUp>();

    public virtual ICollection<Transaction1> Transaction1s { get; set; } = new List<Transaction1>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
