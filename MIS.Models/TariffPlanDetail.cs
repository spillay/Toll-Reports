using System;
using System.Collections.Generic;



namespace MIS.Models
{
    public partial class TariffPlanDetail
    {
        public int TariffPlanId { get; set; }
        public byte TollClassId { get; set; }

        public int TransactionTypeId { get; set; } // Added 13_feb_2026
        public double AmountInclusive { get; set; }
        public double Vat { get; set; }
        public double DiscountAmount { get; set; }
        public double AmountExclusive { get; set; }
        public double VatRate { get; set; }

        public virtual TariffPlan TariffPlan { get; set; }
        public virtual TollClass TollClass { get; set; }
    }
}
