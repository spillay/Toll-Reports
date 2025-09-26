using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class TransactionVehicleCharacteristic
    {
        public byte LaneId { get; set; }
        public long TransactionNumber { get; set; }
        public byte Axle1Count { get; set; }
        public bool Axle1Failed { get; set; }
        public bool EntryLoopTriggered { get; set; }
        public byte Height1Count { get; set; }
        public bool Height1SensorFailed { get; set; }
        public byte Height2Count { get; set; }
        public bool Height2SensorFailed { get; set; }
        public bool HeightBeforeFirstAxle { get; set; }
        public byte UnArmedAxle1Count { get; set; }
        public byte UnArmedHeight1Count { get; set; }
        public byte UnArmedHeight2Count { get; set; }
    }
}
