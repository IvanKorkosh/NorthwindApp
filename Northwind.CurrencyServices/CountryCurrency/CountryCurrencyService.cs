using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

#pragma warning disable CA1031
#pragma warning disable CA1822

namespace Northwind.CurrencyServices.CountryCurrency
{
    /// <summary>
    /// Represents a service that produces info about local currency.
    /// </summary>
    public class CountryCurrencyService : ICountryCurrencyService
    {
        /// <summary>
        /// Gets a info about local currency.
        /// </summary>
        /// <param name="countryName">Name of country.</param>
        /// <returns>Returns <see cref="LocalCurrency"/>.</returns>
        public async Task<LocalCurrency> GetLocalCurrencyByCountry(string countryName)
        {
            var client = new HttpClient();
            var source = await client.GetStringAsync($"https://restcountries.com/v3.1/name/{countryName}");
            client.Dispose();

            JsonElement json = JsonSerializer.Deserialize<JsonElement>(source);

            var result = new LocalCurrency();

            try
            {
                result.CountryName = json[0].GetProperty("name").GetProperty("official").GetString();
            }
            catch (Exception)
            {
                result.CountryName = string.Empty;
            }

            try
            {
                result.CurrencyCode = json[0].GetProperty("currencies").EnumerateObject().First().Name;
            }
            catch (Exception)
            {
                result.CurrencyCode = string.Empty;
            }

            try
            {
                result.CurrencySymbol = json[0].GetProperty("currencies").EnumerateObject().First().Value.GetProperty("symbol").GetString();
            }
            catch (Exception)
            {
                result.CurrencySymbol = string.Empty;
            }

            return result;
        }
    }
}
