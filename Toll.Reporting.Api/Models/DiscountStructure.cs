using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class DiscountStructure
{
    public int DiscountStructureId { get; set; }

    public byte DiscountTypeId { get; set; }

    public DateOnly EffectiveDate { get; set; }

    public virtual ICollection<DiscountStructureDetail> DiscountStructureDetails { get; set; } = new List<DiscountStructureDetail>();

    public virtual DiscountType DiscountType { get; set; } = null!;
}
