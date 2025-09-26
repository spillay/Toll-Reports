using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class Incident
    {
        public Incident()
        {
            LaneIncidents = new HashSet<LaneIncident>();
        }

        public int IncidentId { get; set; }
        public string IncidentCode { get; set; }
        public string Description { get; set; }

        public virtual ICollection<LaneIncident> LaneIncidents { get; set; }
    }
}
