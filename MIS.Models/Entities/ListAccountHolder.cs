using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class ListAccountHolder
{
    public long RegisterUserId { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string CompanyName { get; set; }

    public bool IsPrepaid { get; set; }

    public string Description { get; set; }

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
}
