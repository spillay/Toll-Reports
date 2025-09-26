using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class DiscountType
    {
        public DiscountType()
        {
            DiscountStructures = new HashSet<DiscountStructure>();
            Transactions = new HashSet<Transaction>();
        }

        public byte DiscountTypeId { get; set; }
        public string Description { get; set; }
        public byte CalculationMethodId { get; set; }

        public virtual CalculationMethod CalculationMethod { get; set; }
        public virtual ICollection<DiscountStructure> DiscountStructures { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
