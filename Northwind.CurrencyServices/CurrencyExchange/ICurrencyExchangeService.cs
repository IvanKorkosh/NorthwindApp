using System.Threading.Tasks;

namespace Northwind.CurrencyServices.CurrencyExchange
{
    /// <summary>
    /// Interface that produces currency exchange rate.
    /// </summary>
    public interface ICurrencyExchangeService
    {
        /// <summary>
        /// Get currency exchange rate.
        /// </summary>
        /// <param name="baseCurrency">Base currency.</param>
        /// <param name="exchangeCurrency">Exchange currency.</param>
        /// <returns>Returns <see cref="decimal"/>Exchange rate.</returns>
        Task<decimal> GetCurrencyExchangeRate(string baseCurrency, string exchangeCurrency);
    }
}
