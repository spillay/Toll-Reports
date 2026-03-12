using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class ClassCorrectionType
{
    public byte ClassCorrectionTypeId { get; set; }

    public string Description { get; set; }

    public virtual ICollection<TransactionClassCorrection> TransactionClassCorrections { get; set; } = new List<TransactionClassCorrection>();
}
