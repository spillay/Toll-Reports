using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class RegisteredUserIdentifier
{
    public long RegisteredUserIdentifierId { get; set; }

    public long RegisteredUserId { get; set; }

    public byte IdentifierTypeId { get; set; }

    public byte RegisterdTollClassId { get; set; }

    public string RegisteredIdentifier { get; set; }

    public string NumberPlateDetails { get; set; }

    public bool IsActive { get; set; }

    public bool HotListed { get; set; }

    public DateOnly ActivationDate { get; set; }

    public DateOnly ExpiryDate { get; set; }

    public string Status { get; set; }

    public byte[] RowVersion { get; set; }

    public long SystemUserId { get; set; }

    public virtual IdentifierType IdentifierType { get; set; }

    public virtual RegisteredUser RegisteredUser { get; set; }
}
