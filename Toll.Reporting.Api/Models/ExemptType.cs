using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class ExemptType
{
    public byte ExemptTypeId { get; set; }

    public string ExemptTypeDescription { get; set; } = null!;

    public byte DisplayOrder { get; set; }

    public virtual ICollection<TransactionClassCorrection> TransactionClassCorrections { get; set; } = new List<TransactionClassCorrection>();
}
