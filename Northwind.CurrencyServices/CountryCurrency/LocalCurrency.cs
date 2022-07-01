namespace Northwind.CurrencyServices.CountryCurrency
{
    /// <summary>
    /// Represents a local currency.
    /// </summary>
    public class LocalCurrency
    {
        /// <summary>
        /// Gets or sets a country name.
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// Gets or sets a currency code.
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Gets or sets a currency symbol.
        /// </summary>
        public string CurrencySymbol { get; set; }
    }
}
