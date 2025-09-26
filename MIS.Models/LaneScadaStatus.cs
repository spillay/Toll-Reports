using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class LaneScadaStatus
    {
        public byte LaneId { get; set; }
        public int Ohls { get; set; }
        public string Collector { get; set; }
        public int Printer { get; set; }
        public int Boom { get; set; }
        public int VehiclePresence { get; set; }
        public int Avcloop { get; set; }
        public int AxleCounter { get; set; }
        public int HeightSensor1 { get; set; }
        public int HeightSensor2 { get; set; }
        public int CashLimit { get; set; }
        public int TagReader { get; set; }
        public int Igcamera { get; set; }
        public int Anprcamera { get; set; }
        public int PowerStatus { get; set; }
        public int BatteryStatus { get; set; }
        public int DoorStatus { get; set; }
        public int CurrentHourCounter { get; set; }
        public int PrevHourCounter { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
