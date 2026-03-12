using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class Shift
{
    public byte ShiftId { get; set; }

    public string Description { get; set; }

    public TimeOnly? StartTimeHour { get; set; }

    public TimeOnly? EndTimeHour { get; set; }

    public virtual ICollection<CollectorCashDeclaration> CollectorCashDeclarations { get; set; } = new List<CollectorCashDeclaration>();

    public virtual ICollection<CollectorCashup> CollectorCashups { get; set; } = new List<CollectorCashup>();

    public virtual ICollection<CollectorShiftAssignment> CollectorShiftAssignments { get; set; } = new List<CollectorShiftAssignment>();

    public virtual ICollection<SupervisorCashup> SupervisorCashups { get; set; } = new List<SupervisorCashup>();

    public virtual ICollection<Transaction1> Transaction1s { get; set; } = new List<Transaction1>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
