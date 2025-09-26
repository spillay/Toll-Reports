using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class CollectorCashDeclaration
    {
        public CollectorCashDeclaration()
        {
            CollectorCashDeclarationDenominations = new HashSet<CollectorCashDeclarationDenomination>();
        }

        public long CollectorCashDeclarationId { get; set; }
        public DateTime ShiftDate { get; set; }
        public byte ShiftId { get; set; }
        public long SystemUserId { get; set; }
        public double TotalLocalCurrencyDeclared { get; set; }
        public double TotalForeignCurrenyDeclared { get; set; }
        public double TotalZar { get; set; }
        public double TotalUsd { get; set; }
        public double TotalDeclared { get; set; }
        public DateTime DeclaredAt { get; set; }
        public long VerifiedById { get; set; }
        public long? AllocatedToCollectorCashupId { get; set; }

        public virtual CollectorCashup AllocatedToCollectorCashup { get; set; }
        public virtual Shift Shift { get; set; }
        public virtual SystemUser SystemUser { get; set; }
        public virtual SystemUser VerifiedBy { get; set; }
        public virtual ICollection<CollectorCashDeclarationDenomination> CollectorCashDeclarationDenominations { get; set; }
    }
}
