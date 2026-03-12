using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class ShiftDaySummary
{
    public DateOnly ShiftDate { get; set; }

    public byte ShiftId { get; set; }

    public long SystemUserId { get; set; }

    public long CollectorId { get; set; }

    public double VehiclesXnominalTariff { get; set; }

    public double Discounts { get; set; }

    public double PositiveDiscrepancies { get; set; }

    public double CardSales { get; set; }

    public double AccountRecharges { get; set; }

    public double CollectorRecoveries { get; set; }

    public double PaidViolations { get; set; }

    public double SurplusCash { get; set; }

    public double CashToBank { get; set; }

    public double BankingShortageSurplus { get; set; }

    public double PrepaidTransactions { get; set; }

    public double UnrecoverablePositiveDescrepancies { get; set; }

    public double CollectorCashShortages { get; set; }

    public double CollectorViolations { get; set; }

    public double CollectorUnderClassificaitons { get; set; }

    public double TotalIncome { get; set; }

    public double UnreconciledDiscrepancies { get; set; }
}
