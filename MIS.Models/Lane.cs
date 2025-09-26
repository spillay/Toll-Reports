using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class Lane
    {
        public Lane()
        {
            LaneCameras = new HashSet<LaneCamera>();
            TransactionClassCorrections = new HashSet<TransactionClassCorrection>();
            Transactions = new HashSet<Transaction>();
        }

        public byte LaneId { get; set; }
        public byte VirtualPlazaId { get; set; }
        public string LaneName { get; set; }
        public string LaneCode { get; set; }
        public string PrinterPort { get; set; }
        public string Ufdport { get; set; }
        public string IodigitalPort { get; set; }
        public string Rfidport { get; set; }
        public string FrontCameraIp { get; set; }
        public string SideCameraIp { get; set; }
        public string AnprcameraIp { get; set; }

        public virtual VirtualPlaza VirtualPlaza { get; set; }
        public virtual LaneLastNo LaneLastNo { get; set; }
        public virtual LaneLastTransactionImage LaneLastTransactionImage { get; set; }
        public virtual ICollection<LaneCamera> LaneCameras { get; set; }
        public virtual ICollection<TransactionClassCorrection> TransactionClassCorrections { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
