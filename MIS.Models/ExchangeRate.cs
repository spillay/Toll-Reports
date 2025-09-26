using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class ExchangeRate
    {
        public byte FromCurrencyId { get; set; }
        public byte ToCurrencyId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public double RateOfExchange { get; set; }

        public virtual Currency FromCurrency { get; set; }
        public virtual Currency ToCurrency { get; set; }
    }
}
