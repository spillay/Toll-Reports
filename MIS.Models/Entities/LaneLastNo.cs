using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class LaneLastNo
{
    public byte LaneId { get; set; }

    public long LastTransactionNo { get; set; }

    public long LastInvoiceNo { get; set; }

    public long LastReceiptNo { get; set; }

    public long LastLoginId { get; set; }

    public virtual Lane Lane { get; set; }
}
