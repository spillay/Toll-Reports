using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class Exempt
{
    public DateOnly ReportDate { get; set; }

    public DateTime StartDateTime { get; set; }

    public DateTime EndDateTime { get; set; }

    public int ClassMExemptCount { get; set; }

    public int ClassIExemptCount { get; set; }

    public int ClassIiExemptCount { get; set; }

    public int ClassIiiExemptCount { get; set; }

    public decimal ClassMExemptAmount { get; set; }

    public decimal ClassIExemptAmount { get; set; }

    public decimal ClassIiExemptAmount { get; set; }

    public decimal ClassIiiExemptAmount { get; set; }

    public int TotalExemptCount { get; set; }

    public decimal TotalExemptAmount { get; set; }

    public DateTime CreatedDateTime { get; set; }
}
