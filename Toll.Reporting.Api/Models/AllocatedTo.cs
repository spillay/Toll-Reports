using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class AllocatedTo
{
    public byte AllocatedToId { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<TransactionClassCorrection> TransactionClassCorrections { get; set; } = new List<TransactionClassCorrection>();
}
