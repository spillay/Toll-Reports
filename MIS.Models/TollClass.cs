using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class TollClass
    {
        public TollClass()
        {
            DiscountStructureDetails = new HashSet<DiscountStructureDetail>();
            TariffPlanDetails = new HashSet<TariffPlanDetail>();
            TransactionActualTollClasses = new HashSet<Transaction>();
            TransactionAutomaticTollClasses = new HashSet<Transaction>();
            TransactionClassCorrections = new HashSet<TransactionClassCorrection>();
            TransactionManualTollClasses = new HashSet<Transaction>();
            TransactionRegisteredTollClasses = new HashSet<Transaction>();
        }

        public byte TollClassId { get; set; }
        public string ClassDescription { get; set; }
        public byte DisplayOrder { get; set; }
        public bool SendForValidation { get; set; }

        public virtual ICollection<DiscountStructureDetail> DiscountStructureDetails { get; set; }
        public virtual ICollection<TariffPlanDetail> TariffPlanDetails { get; set; }
        public virtual ICollection<Transaction> TransactionActualTollClasses { get; set; }
        public virtual ICollection<Transaction> TransactionAutomaticTollClasses { get; set; }
        public virtual ICollection<TransactionClassCorrection> TransactionClassCorrections { get; set; }
        public virtual ICollection<Transaction> TransactionManualTollClasses { get; set; }
        public virtual ICollection<Transaction> TransactionRegisteredTollClasses { get; set; }
    }
}
