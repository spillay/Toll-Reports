using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS.Models
{
    public class LaneTransactionImage
    {
        public byte[] ImageBytes { get; set; }
        public string LaneCode { get; set; }
        public byte CameraId { get; set; }
        public long TransactionNumber { get; set; }
        public DateTime TakeAt { get; set; }
    }
}
