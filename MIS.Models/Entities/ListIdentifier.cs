using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class ListIdentifier
{
    public long RegisteredUserIdentifierId { get; set; }

    public long RegisteredUserId { get; set; }

    public string Description { get; set; }

    public string ClassDescription { get; set; }

    public string RegisteredIdentifier { get; set; }

    public string NumberPlateDetails { get; set; }

    public bool IsActive { get; set; }

    public bool HotListed { get; set; }

    public DateOnly ActivationDate { get; set; }

    public DateOnly ExpiryDate { get; set; }

    public string Status { get; set; }
}
