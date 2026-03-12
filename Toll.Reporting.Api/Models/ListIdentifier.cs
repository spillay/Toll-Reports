using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class ListIdentifier
{
    public long RegisteredUserIdentifierId { get; set; }

    public long RegisteredUserId { get; set; }

    public string? Description { get; set; }

    public string? ClassDescription { get; set; }

    public string RegisteredIdentifier { get; set; } = null!;

    public string NumberPlateDetails { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool HotListed { get; set; }

    public DateOnly ActivationDate { get; set; }

    public DateOnly ExpiryDate { get; set; }

    public string Status { get; set; } = null!;
}
