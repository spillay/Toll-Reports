using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

/// <summary>
/// Transactions concluded in teh lanes and transmitted to teh Back office
/// </summary>
public partial class Lane
{
    public byte LaneId { get; set; }

    public byte VirtualPlazaId { get; set; }

    public string LaneName { get; set; } = null!;

    public string LaneCode { get; set; } = null!;

    public string PrinterPort { get; set; } = null!;

    public string Ufdport { get; set; } = null!;

    public string IodigitalPort { get; set; } = null!;

    public string Rfidport { get; set; } = null!;

    public string FrontCameraIp { get; set; } = null!;

    public string SideCameraIp { get; set; } = null!;

    public string AnprcameraIp { get; set; } = null!;

    public string? SmartCardComPort { get; set; }

    public string? Avccomms { get; set; }

    public virtual ICollection<LaneCamera> LaneCameras { get; set; } = new List<LaneCamera>();

    public virtual LaneLastNo? LaneLastNo { get; set; }

    public virtual LaneLastTransactionImage? LaneLastTransactionImage { get; set; }

    public virtual ICollection<Transaction1> Transaction1s { get; set; } = new List<Transaction1>();

    public virtual ICollection<TransactionClassCorrection> TransactionClassCorrections { get; set; } = new List<TransactionClassCorrection>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual VirtualPlaza VirtualPlaza { get; set; } = null!;
}
