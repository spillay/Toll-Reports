using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class TransactionMissing
    {
        public long TransactionMissing1 { get; set; }
        public byte LaneId { get; set; }
        public long StartTransactionNumber { get; set; }
        public long EndTransactionNumber { get; set; }
        public int NumberMissing { get; set; }
        public int NumberReceived { get; set; }
        public bool NoMoreMissing { get; set; }
    }
}
