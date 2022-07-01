using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

#pragma warning disable CA1031

namespace Northwind.CurrencyServices.CurrencyExchange
{
    /// <summary>
    /// Represents a service that produces currency exchange rate.
    /// </summary>
    public class CurrencyExchangeService : ICurrencyExchangeService
    {
        private readonly string accessKey;
        private string source;
        private bool isResponseExist;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyExchangeService"/> class.
        /// </summary>
        /// <param name="accesskey">Access key.</param>
        public CurrencyExchangeService(string accesskey)
        {
            this.accessKey = !string.IsNullOrWhiteSpace(accesskey) ? accesskey : throw new ArgumentException("Access key is invalid.", nameof(accesskey));
        }

        /// <summary>
        /// Get currency exchange rate.
        /// </summary>
        /// <param name="baseCurrency">Base currency.</param>
        /// <param name="exchangeCurrency">Exchange currency.</param>
        /// <returns>Returns <see cref="decimal"/>Exchange rate.</returns>
        public async Task<decimal> GetCurrencyExchangeRate(string baseCurrency, string exchangeCurrency)
        {
            if (!this.isResponseExist)
            {
                await this.GetAllRates();
                this.isResponseExist = true;
            }

            try
            {
                return JsonSerializer.Deserialize<JsonElement>(this.source)
                    .GetProperty("quotes")
                    .EnumerateObject()
                    .First(o => o.Name == $"{baseCurrency}{exchangeCurrency}".ToUpperInvariant())
                    .Value.GetDecimal();
            }
            catch (Exception)
            {
                return 1;
            }
        }

        /// <summary>
        /// Get all exchange rates.
        /// </summary>
        /// <returns>Returns <see cref="Task"/>Quering task.</returns>
        public async Task GetAllRates()
        {
            var client = new HttpClient();
            this.source = await client.GetStringAsync($"http://api.currencylayer.com/live?access_key={this.accessKey}");
            client.Dispose();
        }
    }
}
