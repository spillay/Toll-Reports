using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class Incident
{
    public int IncidentId { get; set; }

    public string IncidentCode { get; set; }

    public string Description { get; set; }

    public virtual ICollection<LaneIncident> LaneIncidents { get; set; } = new List<LaneIncident>();
}
