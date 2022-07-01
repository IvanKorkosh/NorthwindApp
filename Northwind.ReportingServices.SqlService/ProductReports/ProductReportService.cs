using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Northwind.CurrencyServices.CountryCurrency;
using Northwind.CurrencyServices.CurrencyExchange;
using Northwind.ReportingServices.ProductReports;

#pragma warning disable S4457
#pragma warning disable CS1998
#pragma warning disable CA2100

namespace Northwind.ReportingServices.SqlService.ProductReports
{
    /// <summary>
    /// Represents a service that produces product-related reports.
    /// </summary>
    public class ProductReportService : IProductReportService
    {
        private readonly string connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductReportService"/> class.
        /// </summary>
        /// <param name="connectionString">String to connect to data base.</param>
        public ProductReportService(string connectionString)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        /// <summary>
        /// Gets a product report with all current products with local price.
        /// </summary>
        /// /// <param name="countryCurrencyService">Service to provide country data.</param>
        /// /// <param name="currencyExchangeService">Service to provide exchange rate.</param>
        /// <returns>Returns <see cref="ProductReport{T}"/>.</returns>
        public async Task<ProductReport<ProductLocalPrice>> GetCurrentProductsWithLocalCurrencyReport(
            ICountryCurrencyService countryCurrencyService,
            ICurrencyExchangeService currencyExchangeService)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a product report with all current products.
        /// </summary>
        /// <returns>Returns <see cref="ProductReport{T}"/>.</returns>
        public async Task<ProductReport<ProductPrice>> GetCurrentProductsReport()
        {
            var query = "SELECT ProductName, UnitPrice FROM Products WHERE Discontinued = 0 ORDER BY ProductName";
            return await this.GetResponse(query);
        }

        /// <summary>
        /// Gets a product report with most expensive products.
        /// </summary>
        /// <param name="count">Items count.</param>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        public async Task<ProductReport<ProductPrice>> GetMostExpensiveProductsReport(int count)
        {
            var query = $"SELECT TOP({count}) ProductName, UnitPrice FROM Products ORDER BY UnitPrice DESC";
            return await this.GetResponse(query);
        }

        /// <summary>
        /// Gets a product report with products which price less then given price.
        /// </summary>
        /// /// <param name="price">Max price.</param>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        public async Task<ProductReport<ProductPrice>> GetPriceLessThenProductsReport(decimal price)
        {
            var query = $"SELECT ProductName, UnitPrice FROM Products WHERE UnitPrice < {price}";
            return await this.GetResponse(query);
        }

        /// <summary>
        /// Gets a product report with products which price in given interval.
        /// </summary>
        /// /// <param name="leftPrice">Left price.</param>
        /// /// <param name="rightPrice">Right price.</param>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        public async Task<ProductReport<ProductPrice>> GetPriceBetweenProductsReport(decimal leftPrice, decimal rightPrice)
        {
            var query = $"SELECT ProductName, UnitPrice FROM Products WHERE UnitPrice >= {leftPrice} AND UnitPrice <= {rightPrice}";
            return await this.GetResponse(query);
        }

        /// <summary>
        /// Gets a product report with products which price above average.
        /// </summary>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        public async Task<ProductReport<ProductPrice>> GetPriceAboveAverageProductsReport()
        {
            var query = $"SELECT ProductName, UnitPrice FROM Products WHERE UnitPrice > (SELECT AVG(UnitPrice)FROM Products)";
            return await this.GetResponse(query);
        }

        /// <summary>
        /// Gets a product report with products in deficit.
        /// </summary>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        public async Task<ProductReport<ProductPrice>> GetUnitsInStockDeficitReport()
        {
            var query = $"SELECT ProductName, UnitPrice FROM Products WHERE UnitsInStock < UnitsOnOrder";
            return await this.GetResponse(query);
        }

        /// <summary>
        /// Gets a product report with most cheap products.
        /// </summary>
        /// /// <param name="count">Items count.</param>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        public async Task<ProductReport<ProductPrice>> GetMostCheapProductsReport(int count)
        {
            var query = $"SELECT TOP({count}) ProductName, UnitPrice FROM Products ORDER BY UnitPrice";
            return await this.GetResponse(query);
        }

        /// <summary>
        /// Gets a product report with largest category.
        /// </summary>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        public async Task<ProductReport<ProductPrice>> GetLargestCategoryProductsReport()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a product report with most expensive category.
        /// </summary>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        public async Task<ProductReport<ProductPrice>> GetMostExpensiveCategoryProductsReport()
        {
            throw new NotImplementedException();
        }

        private async Task<ProductReport<ProductPrice>> GetResponse(string query)
        {
            var sqlConnection = new SqlConnection(this.connectionString);
            sqlConnection.Open();

            var command = new SqlCommand(query, sqlConnection);
            var dataReader = await command.ExecuteReaderAsync();
            var result = new ProductReport<ProductPrice>();

            while (dataReader.Read())
            {
                result.Products.Add(new ProductPrice()
                {
                    Name = Convert.ToString(dataReader["ProductName"], CultureInfo.InvariantCulture),
                    Price = Convert.ToDecimal(dataReader["UnitPrice"], CultureInfo.InvariantCulture),
                });
            }

            command.Dispose();
            sqlConnection.Close();
            return result;
        }
    }
}
