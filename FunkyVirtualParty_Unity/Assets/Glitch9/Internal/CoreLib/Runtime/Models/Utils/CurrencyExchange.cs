using System;
using System.Collections.Generic;

namespace Glitch9
{
    public class CurrencyExchange
    {
        private readonly Dictionary<CurrencyCode, ExchangeRate> _rates = new();

        public CurrencyExchange()
        {
            LoadRates(CurrencyUtil.GetExchangeRates());
        }

        public void LoadRates(List<ExchangeRate> currencyRates)
        {
            foreach (ExchangeRate rate in currencyRates)
            {
                _rates[rate.Currency] = rate;
            }
        }

        public double Convert(double amount, CurrencyCode fromCurrency, CurrencyCode toCurrency)
        {
            if (fromCurrency == toCurrency)
            {
                return amount;
            }

            if (!_rates.ContainsKey(fromCurrency) || !_rates.ContainsKey(toCurrency))
            {
                throw new Exception("Currency rate not found.");
            }

            double amountInUSD = amount * _rates[fromCurrency].ToUSD;
            return amountInUSD * _rates[toCurrency].FromUSD;
        }
    }
}