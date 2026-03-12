using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class Discount
{
    public DateOnly ReportDate { get; set; }

    public DateTime StartDateTime { get; set; }

    public DateTime EndDateTime { get; set; }

    public int ClassMAnonymousCount { get; set; }

    public int ClassIAnonymousCount { get; set; }

    public int ClassIiAnonymousCount { get; set; }

    public int ClassIiiAnonymousCount { get; set; }

    public decimal ClassMAnonymousAmount { get; set; }

    public decimal ClassIAnonymousAmount { get; set; }

    public decimal ClassIiAnonymousAmount { get; set; }

    public decimal ClassIiiAnonymousAmount { get; set; }

    public int ClassMStaffCount { get; set; }

    public int ClassIStaffCount { get; set; }

    public int ClassIiStaffCount { get; set; }

    public int ClassIiiStaffCount { get; set; }

    public decimal ClassMStaffAmount { get; set; }

    public decimal ClassIStaffAmount { get; set; }

    public decimal ClassIiStaffAmount { get; set; }

    public decimal ClassIiiStaffAmount { get; set; }

    public decimal ClassMIndividualAmount { get; set; }

    public decimal ClassIIndividualAmount { get; set; }

    public decimal ClassIiIndividualAmount { get; set; }

    public decimal ClassIiiIndividualAmount { get; set; }

    public decimal ClassMCorporateAmount { get; set; }

    public decimal ClassICorporateAmount { get; set; }

    public decimal ClassIiCorporateAmount { get; set; }

    public decimal ClassIiiCorporateAmount { get; set; }

    public int TotalDiscountCount { get; set; }

    public decimal TotalDiscountAmount { get; set; }

    public DateTime CreatedDateTime { get; set; }
}
