using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class DiscountType
{
    public byte DiscountTypeId { get; set; }

    public string Description { get; set; } = null!;

    public byte CalculationMethodId { get; set; }

    public virtual CalculationMethod CalculationMethod { get; set; } = null!;

    public virtual ICollection<DiscountStructure> DiscountStructures { get; set; } = new List<DiscountStructure>();

    public virtual ICollection<Transaction1> Transaction1s { get; set; } = new List<Transaction1>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
