using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class TollClass1
{
    public byte TollClassId { get; set; }

    public string ClassDescription { get; set; }

    public byte DisplayOrder { get; set; }

    public bool SendForValidation { get; set; }
}
