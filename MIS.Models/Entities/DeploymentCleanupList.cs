using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class DeploymentCleanupList
{
    public string SchemaName { get; set; }

    public string TableName { get; set; }

    public string CleanupMethod { get; set; }

    public bool ResetIdentity { get; set; }
}
