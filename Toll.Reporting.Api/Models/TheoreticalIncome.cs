using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class TheoreticalIncome
{
    public DateOnly ReportDate { get; set; }

    public DateTime StartDateTime { get; set; }

    public DateTime EndDateTime { get; set; }

    public string Metric { get; set; } = null!;

    public decimal? ClassM { get; set; }

    public decimal? ClassI { get; set; }

    public decimal? ClassIi { get; set; }

    public decimal? ClassIii { get; set; }

    public decimal? Total { get; set; }

    public DateTime CreatedDateTime { get; set; }
}
