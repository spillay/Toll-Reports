using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class DeploymentCleanupList
{
    public string SchemaName { get; set; } = null!;

    public string TableName { get; set; } = null!;

    public string CleanupMethod { get; set; } = null!;

    public bool ResetIdentity { get; set; }
}
