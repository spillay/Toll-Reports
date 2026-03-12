using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class LaneIncident
{
    public long LaneIncidentId { get; set; }

    public byte LaneId { get; set; }

    public int IncidentId { get; set; }

    public DateTime OccurredAt { get; set; }

    public long? TransactionNumber { get; set; }

    public virtual Incident Incident { get; set; } = null!;
}
