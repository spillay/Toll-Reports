using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class SystemUser
{
    public long SystemUserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool IsLocked { get; set; }

    public bool RequiresPasswordReset { get; set; }

    public DateOnly ActivationDate { get; set; }

    public bool IsActive { get; set; }

    public DateOnly? PasswordExpiryDate { get; set; }

    public bool PasswordExpires { get; set; }

    public short PasswordDaysToExpire { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public virtual ICollection<CollectorCashDeclaration> CollectorCashDeclarationSystemUsers { get; set; } = new List<CollectorCashDeclaration>();

    public virtual ICollection<CollectorCashDeclaration> CollectorCashDeclarationVerifiedBies { get; set; } = new List<CollectorCashDeclaration>();

    public virtual ICollection<CollectorCashupShortagePayment> CollectorCashupShortagePayments { get; set; } = new List<CollectorCashupShortagePayment>();

    public virtual ICollection<CollectorCashup> CollectorCashups { get; set; } = new List<CollectorCashup>();

    public virtual ICollection<CollectorShiftAssignment> CollectorShiftAssignments { get; set; } = new List<CollectorShiftAssignment>();

    public virtual ICollection<SupervisorCashup> SupervisorCashupSystemUsers { get; set; } = new List<SupervisorCashup>();

    public virtual ICollection<SupervisorCashup> SupervisorCashupVerifiedBies { get; set; } = new List<SupervisorCashup>();

    public virtual ICollection<SystemUserRole> SystemUserRoles { get; set; } = new List<SystemUserRole>();

    public virtual ICollection<Transaction1> Transaction1s { get; set; } = new List<Transaction1>();
}
