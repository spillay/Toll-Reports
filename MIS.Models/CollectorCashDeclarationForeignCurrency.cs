using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class CollectorCashDeclarationForeignCurrency
    {
        public long CollectorCashDeclarationId { get; set; }
        public byte CurrencyId { get; set; }
        public double ForeignCurrencyReceived { get; set; }
        public double ExchangeRateUsed { get; set; }
        public double AmountInLocalCurrency { get; set; }
    }
}
