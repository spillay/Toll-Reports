using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class TollClass1
{
    public byte TollClassId { get; set; }

    public string ClassDescription { get; set; } = null!;

    public byte DisplayOrder { get; set; }

    public bool SendForValidation { get; set; }
}
