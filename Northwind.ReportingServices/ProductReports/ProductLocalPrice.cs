namespace Northwind.ReportingServices.ProductReports
{
    /// <summary>
    /// Represents a product local price.
    /// </summary>
    public class ProductLocalPrice
    {
        /// <summary>
        /// Gets or sets a product name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a product price.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets a product country.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets a product local price.
        /// </summary>
        public decimal LocalPrice { get; set; }

        /// <summary>
        /// Gets or sets a product currency symbol.
        /// </summary>
        public string CurrencySymbol { get; set; }
    }
}
