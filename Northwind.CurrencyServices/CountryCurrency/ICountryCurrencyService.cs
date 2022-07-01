using System.Threading.Tasks;

namespace Northwind.CurrencyServices.CountryCurrency
{
    /// <summary>
    /// Interface that produces info about local currency.
    /// </summary>
    public interface ICountryCurrencyService
    {
        /// <summary>
        /// Gets a info about local currency.
        /// </summary>
        /// <param name="countryName">Name of country.</param>
        /// <returns>Returns <see cref="LocalCurrency"/>.</returns>
        Task<LocalCurrency> GetLocalCurrencyByCountry(string countryName);
    }
}
