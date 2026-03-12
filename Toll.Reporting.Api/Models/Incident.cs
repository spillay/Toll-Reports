using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class Incident
{
    public int IncidentId { get; set; }

    public string IncidentCode { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<LaneIncident> LaneIncidents { get; set; } = new List<LaneIncident>();
}
