using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class TollClass
{
    public byte TollClassId { get; set; }

    public string ClassDescription { get; set; } = null!;

    public byte DisplayOrder { get; set; }

    public bool SendForValidation { get; set; }

    public virtual ICollection<DiscountStructureDetail> DiscountStructureDetails { get; set; } = new List<DiscountStructureDetail>();

    public virtual ICollection<TariffPlanDetail> TariffPlanDetails { get; set; } = new List<TariffPlanDetail>();

    public virtual ICollection<Transaction1> Transaction1ActualTollClasses { get; set; } = new List<Transaction1>();

    public virtual ICollection<Transaction1> Transaction1AutomaticTollClasses { get; set; } = new List<Transaction1>();

    public virtual ICollection<Transaction1> Transaction1ManualTollClasses { get; set; } = new List<Transaction1>();

    public virtual ICollection<Transaction1> Transaction1RegisteredTollClasses { get; set; } = new List<Transaction1>();

    public virtual ICollection<Transaction> TransactionActualTollClasses { get; set; } = new List<Transaction>();

    public virtual ICollection<Transaction> TransactionAutomaticTollClasses { get; set; } = new List<Transaction>();

    public virtual ICollection<TransactionClassCorrection> TransactionClassCorrections { get; set; } = new List<TransactionClassCorrection>();

    public virtual ICollection<Transaction> TransactionManualTollClasses { get; set; } = new List<Transaction>();

    public virtual ICollection<Transaction> TransactionRegisteredTollClasses { get; set; } = new List<Transaction>();
}
