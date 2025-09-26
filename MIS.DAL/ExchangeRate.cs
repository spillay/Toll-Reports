using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIS.Models;

namespace MIS.DAL
{
    public class ExchangeRate
    {
        public List<Models.ExchangeRate> GetLatestTo(byte CurrencyId, DateTime EffectiveDate)
        {
            using (var db = new Models.MISDBContext())
            {
                var exchangeRates = db.ExchangeRates.Where(y => y.ToCurrencyId == CurrencyId).GroupBy(x => new { x.FromCurrencyId })
                    .Select(r => new Models.ExchangeRate
                    {
                        ToCurrencyId = CurrencyId,
                        FromCurrencyId = r.Key.FromCurrencyId,
                        EffectiveDate = db.ExchangeRates.Where(z => z.FromCurrencyId == r.Key.FromCurrencyId && z.ToCurrencyId == CurrencyId && z.EffectiveDate <= EffectiveDate.Date).Max(x => x.EffectiveDate),
                        RateOfExchange = db.ExchangeRates.Where(z => z.FromCurrencyId == r.Key.FromCurrencyId && z.ToCurrencyId == CurrencyId &&
                            z.EffectiveDate == db.ExchangeRates.Where(z => z.FromCurrencyId == r.Key.FromCurrencyId && z.ToCurrencyId == CurrencyId && z.EffectiveDate <= EffectiveDate.Date).Max(x => x.EffectiveDate)).Select(z => z.RateOfExchange).First()
                    });

                return exchangeRates.ToList();
            }
        }
    }
}
