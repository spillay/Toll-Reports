using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class Shift
    {
        public Shift()
        {
            CollectorCashDeclarations = new HashSet<CollectorCashDeclaration>();
            CollectorCashups = new HashSet<CollectorCashup>();
            CollectorShiftAssignments = new HashSet<CollectorShiftAssignment>();
            SupervisorCashups = new HashSet<SupervisorCashup>();
            Transactions = new HashSet<Transaction>();
        }

        public byte ShiftId { get; set; }
        public string Description { get; set; }
        public byte StartTimeHour { get; set; }
        public byte EndTimeHour { get; set; }

        public virtual ICollection<CollectorCashDeclaration> CollectorCashDeclarations { get; set; }
        public virtual ICollection<CollectorCashup> CollectorCashups { get; set; }
        public virtual ICollection<CollectorShiftAssignment> CollectorShiftAssignments { get; set; }
        public virtual ICollection<SupervisorCashup> SupervisorCashups { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
