using System.Threading.Tasks;
using Northwind.CurrencyServices.CountryCurrency;
using Northwind.CurrencyServices.CurrencyExchange;

namespace Northwind.ReportingServices.ProductReports
{
    /// <summary>
    /// Interface that produces product-related reports.
    /// </summary>
    public interface IProductReportService
    {
        /// <summary>
        /// Gets a product report with all current products with local price.
        /// </summary>
        /// <param name="countryCurrencyService">Service to provide country data.</param>
        /// <param name="currencyExchangeService">Service to provide exchange rate.</param>
        /// <returns>Returns <see cref="ProductReport{T}"/>.</returns>
        Task<ProductReport<ProductLocalPrice>> GetCurrentProductsWithLocalCurrencyReport(
            ICountryCurrencyService countryCurrencyService,
            ICurrencyExchangeService currencyExchangeService);

        /// <summary>
        /// Gets a product report with all current products.
        /// </summary>
        /// <returns>Returns <see cref="ProductReport{T}"/>.</returns>
        Task<ProductReport<ProductPrice>> GetCurrentProductsReport();

        /// <summary>
        /// Gets a product report with most expensive products.
        /// </summary>
        /// <param name="count">Items count.</param>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        Task<ProductReport<ProductPrice>> GetMostExpensiveProductsReport(int count);

        /// <summary>
        /// Gets a product report with products which price less then given price.
        /// </summary>
        /// /// <param name="price">Max price.</param>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        Task<ProductReport<ProductPrice>> GetPriceLessThenProductsReport(decimal price);

        /// <summary>
        /// Gets a product report with products which price in given interval.
        /// </summary>
        /// /// <param name="leftPrice">Left price.</param>
        /// /// <param name="rightPrice">Right price.</param>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        Task<ProductReport<ProductPrice>> GetPriceBetweenProductsReport(decimal leftPrice, decimal rightPrice);

        /// <summary>
        /// Gets a product report with products which price above average.
        /// </summary>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        Task<ProductReport<ProductPrice>> GetPriceAboveAverageProductsReport();

        /// <summary>
        /// Gets a product report with products in deficit.
        /// </summary>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        Task<ProductReport<ProductPrice>> GetUnitsInStockDeficitReport();

        /// <summary>
        /// Gets a product report with most cheap products.
        /// </summary>
        /// /// <param name="count">Items count.</param>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        Task<ProductReport<ProductPrice>> GetMostCheapProductsReport(int count);

        /// <summary>
        /// Gets a product report with largest category.
        /// </summary>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        Task<ProductReport<ProductPrice>> GetLargestCategoryProductsReport();

        /// <summary>
        /// Gets a product report with most expensive category.
        /// </summary>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        Task<ProductReport<ProductPrice>> GetMostExpensiveCategoryProductsReport();
    }
}
