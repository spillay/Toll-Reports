using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class DiscountType
{
    public byte DiscountTypeId { get; set; }

    public string Description { get; set; }

    public byte CalculationMethodId { get; set; }

    public virtual CalculationMethod CalculationMethod { get; set; }

    public virtual ICollection<DiscountStructure> DiscountStructures { get; set; } = new List<DiscountStructure>();

    public virtual ICollection<Transaction1> Transaction1s { get; set; } = new List<Transaction1>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
