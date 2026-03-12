using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class LaneHourlyAudit
{
    public byte LaneId { get; set; }

    public DateOnly CalendarDate { get; set; }

    public byte Hour { get; set; }

    public long StartTransactionNumber { get; set; }

    public long EndTransactionNumber { get; set; }

    public int TransactionCount { get; set; }
}
