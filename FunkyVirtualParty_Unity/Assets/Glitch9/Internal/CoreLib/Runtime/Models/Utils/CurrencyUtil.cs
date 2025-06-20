using System.Collections.Generic;

namespace Glitch9
{
    public static class CurrencyUtil
    {
        /// <summary>
        /// Rates are based on the date and time below:
        /// <para>- Updated Jun 27, 2024 00:43 UTC</para>
        /// </summary>
        /// <returns></returns>
        public static List<ExchangeRate> GetExchangeRates()
        {
            return new List<ExchangeRate>
            {
                new() { Currency = CurrencyCode.USD, ToUSD = 1.00f, FromUSD = 1.00f },
                new() { Currency = CurrencyCode.EUR, ToUSD = 0.936443f, FromUSD = 1.067870f },
                new() { Currency = CurrencyCode.GBP, ToUSD = 0.792748f, FromUSD = 1.261435f },
                new() { Currency = CurrencyCode.INR, ToUSD = 83.586469f, FromUSD = 0.011964f },
                new() { Currency = CurrencyCode.AUD, ToUSD = 1.505727f, FromUSD = 0.664131f },
                new() { Currency = CurrencyCode.CAD, ToUSD = 1.371085f, FromUSD = 0.729350f },
                new() { Currency = CurrencyCode.SGD, ToUSD = 1.359339f, FromUSD = 0.735652f },
                new() { Currency = CurrencyCode.CHF, ToUSD = 0.897359f, FromUSD = 1.114381f },
                new() { Currency = CurrencyCode.MYR, ToUSD = 4.721315f, FromUSD = 0.211805f },
                new() { Currency = CurrencyCode.JPY, ToUSD = 160.656574f, FromUSD = 0.006224f },
                new() { Currency = CurrencyCode.CNY, ToUSD = 7.266506f, FromUSD = 0.137618f },
                new() { Currency = CurrencyCode.ARS, ToUSD = 910.953779f, FromUSD = 0.001098f },
                new() { Currency = CurrencyCode.BHD, ToUSD = 0.376000f, FromUSD = 2.659574f },
                new() { Currency = CurrencyCode.BWP, ToUSD = 13.507524f, FromUSD = 0.074033f },
                new() { Currency = CurrencyCode.BRL, ToUSD = 5.523650f, FromUSD = 0.181040f },
                new() { Currency = CurrencyCode.BND, ToUSD = 1.359339f, FromUSD = 0.735652f },
                new() { Currency = CurrencyCode.BGN, ToUSD = 1.831524f, FromUSD = 0.545993f },
                new() { Currency = CurrencyCode.CLP, ToUSD = 946.783846f, FromUSD = 0.001056f },
                new() { Currency = CurrencyCode.COP, ToUSD = 4145.375538f, FromUSD = 0.000241f },
                new() { Currency = CurrencyCode.CZK, ToUSD = 23.332331f, FromUSD = 0.042859f },
                new() { Currency = CurrencyCode.DKK, ToUSD = 6.985133f, FromUSD = 0.143161f },
                new() { Currency = CurrencyCode.AED, ToUSD = 3.672500f, FromUSD = 0.272294f },
                new() { Currency = CurrencyCode.HKD, ToUSD = 7.808490f, FromUSD = 0.128066f },
                new() { Currency = CurrencyCode.HUF, ToUSD = 371.788899f, FromUSD = 0.002690f },
                new() { Currency = CurrencyCode.ISK, ToUSD = 139.444782f, FromUSD = 0.007171f },
                new() { Currency = CurrencyCode.IDR, ToUSD = 16434.874423f, FromUSD = 0.000061f },
                new() { Currency = CurrencyCode.IRR, ToUSD = 42290.211572f, FromUSD = 0.000024f },
                new() { Currency = CurrencyCode.ILS, ToUSD = 3.753516f, FromUSD = 0.266417f },
                new() { Currency = CurrencyCode.KZT, ToUSD = 464.021496f, FromUSD = 0.002155f },
                new() { Currency = CurrencyCode.KWD, ToUSD = 0.306855f, FromUSD = 3.258868f },
                new() { Currency = CurrencyCode.LYD, ToUSD = 4.860020f, FromUSD = 0.205760f },
                new() { Currency = CurrencyCode.MUR, ToUSD = 46.997484f, FromUSD = 0.021278f },
                new() { Currency = CurrencyCode.MXN, ToUSD = 18.330546f, FromUSD = 0.054554f },
                new() { Currency = CurrencyCode.NPR, ToUSD = 133.801040f, FromUSD = 0.007474f },
                new() { Currency = CurrencyCode.NZD, ToUSD = 1.647192f, FromUSD = 0.607094f },
                new() { Currency = CurrencyCode.NOK, ToUSD = 10.690043f, FromUSD = 0.093545f },
                new() { Currency = CurrencyCode.OMR, ToUSD = 0.384970f, FromUSD = 2.597605f },
                new() { Currency = CurrencyCode.PKR, ToUSD = 279.041436f, FromUSD = 0.003584f },
                new() { Currency = CurrencyCode.PHP, ToUSD = 58.879940f, FromUSD = 0.016984f },
                new() { Currency = CurrencyCode.PLN, ToUSD = 4.041663f, FromUSD = 0.247423f },
                new() { Currency = CurrencyCode.QAR, ToUSD = 3.640000f, FromUSD = 0.274725f },
                new() { Currency = CurrencyCode.RON, ToUSD = 4.660641f, FromUSD = 0.214563f },
                new() { Currency = CurrencyCode.RUB, ToUSD = 90.458408f, FromUSD = 0.011055f },
                new() { Currency = CurrencyCode.SAR, ToUSD = 3.750000f, FromUSD = 0.266667f },
                new() { Currency = CurrencyCode.ZAR, ToUSD = 18.175395f, FromUSD = 0.055019f },
                new() { Currency = CurrencyCode.KRW, ToUSD = 1392.231385f, FromUSD = 0.000718f },
                new() { Currency = CurrencyCode.LKR, ToUSD = 305.531173f, FromUSD = 0.003273f },
                new() { Currency = CurrencyCode.SEK, ToUSD = 10.584672f, FromUSD = 0.094476f },
                new() { Currency = CurrencyCode.TWD, ToUSD = 32.604256f, FromUSD = 0.030671f },
                new() { Currency = CurrencyCode.THB, ToUSD = 36.976521f, FromUSD = 0.027044f },
                new() { Currency = CurrencyCode.TTD, ToUSD = 6.783221f, FromUSD = 0.147423f },
                new() { Currency = CurrencyCode.TRY, ToUSD = 32.855011f, FromUSD = 0.030437f },
                new() { Currency = CurrencyCode.VES, ToUSD = 3633713.700885f, FromUSD = 0.000000f },
            };
        }

        public static string GetSymbol(CurrencyCode currencyCode)
        {
            return currencyCode switch
            {
                CurrencyCode.USD => "$",
                CurrencyCode.EUR => "€",
                CurrencyCode.GBP => "£",
                CurrencyCode.JPY => "¥",
                CurrencyCode.CAD => "C$",
                CurrencyCode.AUD => "A$",
                CurrencyCode.CHF => "CHF",
                CurrencyCode.CNY => "¥",
                CurrencyCode.HKD => "HK$",
                CurrencyCode.SGD => "S$",
                CurrencyCode.NZD => "NZ$",
                CurrencyCode.KRW => "₩",
                CurrencyCode.INR => "₹",
                CurrencyCode.RUB => "₽",
                CurrencyCode.BRL => "R$",
                CurrencyCode.ZAR => "R",
                CurrencyCode.SEK => "kr",
                CurrencyCode.NOK => "kr",
                CurrencyCode.DKK => "kr",
                CurrencyCode.PLN => "zł",
                CurrencyCode.TRY => "₺",
                CurrencyCode.THB => "฿",
                CurrencyCode.MYR => "RM",
                CurrencyCode.IDR => "Rp",
                CurrencyCode.PHP => "₱",
                CurrencyCode.VND => "₫",
                _ => "?"
            };
        }
    }
}
