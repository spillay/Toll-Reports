using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class SystemUser
    {
        public SystemUser()
        {
            CollectorCashDeclarationSystemUsers = new HashSet<CollectorCashDeclaration>();
            CollectorCashDeclarationVerifiedBies = new HashSet<CollectorCashDeclaration>();
            CollectorCashupShortagePayments = new HashSet<CollectorCashupShortagePayment>();
            CollectorCashups = new HashSet<CollectorCashup>();
            CollectorShiftAssignments = new HashSet<CollectorShiftAssignment>();
            SupervisorCashupSystemUsers = new HashSet<SupervisorCashup>();
            SupervisorCashupVerifiedBies = new HashSet<SupervisorCashup>();
            SystemUserRoles = new HashSet<SystemUserRole>();
            Transactions = new HashSet<Transaction>();
        }

        public long SystemUserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsLocked { get; set; }
        public bool RequiresPasswordReset { get; set; }
        public DateTime ActivationDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime? PasswordExpiryDate { get; set; }
        public bool PasswordExpires { get; set; }
        public short PasswordDaysToExpire { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual ICollection<CollectorCashDeclaration> CollectorCashDeclarationSystemUsers { get; set; }
        public virtual ICollection<CollectorCashDeclaration> CollectorCashDeclarationVerifiedBies { get; set; }
        public virtual ICollection<CollectorCashupShortagePayment> CollectorCashupShortagePayments { get; set; }
        public virtual ICollection<CollectorCashup> CollectorCashups { get; set; }
        public virtual ICollection<CollectorShiftAssignment> CollectorShiftAssignments { get; set; }
        public virtual ICollection<SupervisorCashup> SupervisorCashupSystemUsers { get; set; }
        public virtual ICollection<SupervisorCashup> SupervisorCashupVerifiedBies { get; set; }
        public virtual ICollection<SystemUserRole> SystemUserRoles { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
