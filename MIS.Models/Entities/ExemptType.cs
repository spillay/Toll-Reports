using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class ExemptType
{
    public byte ExemptTypeId { get; set; }

    public string ExemptTypeDescription { get; set; }

    public byte DisplayOrder { get; set; }

    public virtual ICollection<TransactionClassCorrection> TransactionClassCorrections { get; set; } = new List<TransactionClassCorrection>();
}
