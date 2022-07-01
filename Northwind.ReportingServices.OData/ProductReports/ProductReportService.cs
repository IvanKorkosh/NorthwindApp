using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Threading.Tasks;
using Northwind.CurrencyServices.CountryCurrency;
using Northwind.CurrencyServices.CurrencyExchange;
using Northwind.ReportingServices.ProductReports;
using NorthwindModel;

#pragma warning disable S4457

namespace Northwind.ReportingServices.OData.ProductReports
{
    /// <summary>
    /// Represents a service that produces product-related reports.
    /// </summary>
    public class ProductReportService : IProductReportService
    {
        private readonly NorthwindModel.NorthwindEntities entities;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductReportService"/> class.
        /// </summary>
        /// <param name="northwindServiceUri">An URL to Northwind OData service.</param>
        public ProductReportService(Uri northwindServiceUri)
        {
            this.entities = new NorthwindModel.NorthwindEntities(northwindServiceUri ?? throw new ArgumentNullException(nameof(northwindServiceUri)));
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
            if (countryCurrencyService is null)
            {
                throw new ArgumentNullException(nameof(countryCurrencyService));
            }

            if (currencyExchangeService is null)
            {
                throw new ArgumentNullException(nameof(currencyExchangeService));
            }

            var result = new ProductReport<ProductLocalPrice>();
            var localCurrencyTasks = new List<(Task<LocalCurrency> localCurrency, Product product)>();

            var response = await this.GetFullResponse(this.entities.Products.Expand("Supplier"));
            foreach (var product in response.Products)
            {
                localCurrencyTasks.Add((countryCurrencyService.GetLocalCurrencyByCountry(product.Supplier.Country), product));
            }

            foreach (var task in localCurrencyTasks)
            {
                var localCurrency = await task.localCurrency;

                var rate = await currencyExchangeService.GetCurrencyExchangeRate("USD", localCurrency.CurrencyCode);

                result.Products.Add(new ProductLocalPrice()
                {
                    Name = task.product.ProductName,
                    Price = (decimal)task.product.UnitPrice,
                    Country = localCurrency.CountryName,
                    LocalPrice = (decimal)task.product.UnitPrice * rate,
                    CurrencySymbol = localCurrency.CurrencySymbol,
                });
            }

            return result;
        }

        /// <summary>
        /// Gets a product report with all current products.
        /// </summary>
        /// <returns>Returns <see cref="ProductReport{T}"/>.</returns>
        public async Task<ProductReport<ProductPrice>> GetCurrentProductsReport()
        {
            var query = (DataServiceQuery<ProductPrice>)(
                from p in this.entities.Products
                where !p.Discontinued
                orderby p.ProductName
                select new ProductPrice
                {
                    Name = p.ProductName,
                    Price = p.UnitPrice ?? 0,
                });

            return await this.GetFullResponse(query);
        }

        /// <summary>
        /// Gets a product report with most expensive products.
        /// </summary>
        /// <param name="count">Items count.</param>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        public async Task<ProductReport<ProductPrice>> GetMostExpensiveProductsReport(int count)
        {
            var query = (DataServiceQuery<ProductPrice>)this.entities.Products
                .Where(p => p.UnitPrice != null)
                .OrderByDescending(p => p.UnitPrice.Value)
                .Take(count)
                .Select(p => new ProductPrice { Name = p.ProductName, Price = p.UnitPrice ?? 0 });

            return await this.GetFullResponse(query);
        }

        /// <summary>
        /// Gets a product report with products which price less then given price.
        /// </summary>
        /// /// <param name="price">Max price.</param>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        public async Task<ProductReport<ProductPrice>> GetPriceLessThenProductsReport(decimal price)
        {
            var query = (DataServiceQuery<ProductPrice>)this.entities.Products
                .Where(p => p.UnitPrice < price)
                .Select(p => new ProductPrice { Name = p.ProductName, Price = p.UnitPrice ?? 0 });

            return await this.GetFullResponse(query);
        }

        /// <summary>
        /// Gets a product report with products which price in given interval.
        /// </summary>
        /// /// <param name="leftPrice">Left price.</param>
        /// /// <param name="rightPrice">Right price.</param>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        public async Task<ProductReport<ProductPrice>> GetPriceBetweenProductsReport(decimal leftPrice, decimal rightPrice)
        {
            var query = (DataServiceQuery<ProductPrice>)this.entities.Products
                .Where(p => p.UnitPrice <= rightPrice && p.UnitPrice >= leftPrice)
                .Select(p => new ProductPrice { Name = p.ProductName, Price = p.UnitPrice ?? 0 });

            return await this.GetFullResponse(query);
        }

        /// <summary>
        /// Gets a product report with products which price above average.
        /// </summary>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        public async Task<ProductReport<ProductPrice>> GetPriceAboveAverageProductsReport()
        {
            var response = (await this.GetCurrentProductsReport()).Products;
            var average = response.Average(p => p.Price);

            return new ProductReport<ProductPrice>(response.Where(p => p.Price > average));
        }

        /// <summary>
        /// Gets a product report with products in deficit.
        /// </summary>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        public async Task<ProductReport<ProductPrice>> GetUnitsInStockDeficitReport()
        {
            var query = (DataServiceQuery<ProductPrice>)this.entities.Products
                .Where(p => p.UnitPrice != null)
                .Where(p => p.UnitsInStock < p.UnitsOnOrder)
                .Select(p => new ProductPrice { Name = p.ProductName, Price = p.UnitPrice ?? 0 });

            return await this.GetFullResponse(query);
        }

        /// <summary>
        /// Gets a product report with most cheap products.
        /// </summary>
        /// /// <param name="count">Items count.</param>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        public async Task<ProductReport<ProductPrice>> GetMostCheapProductsReport(int count)
        {
            var query = (DataServiceQuery<ProductPrice>)this.entities.Products
                .Where(p => p.UnitPrice != null)
                .OrderBy(p => p.UnitPrice.Value)
                .Take(count)
                .Select(p => new ProductPrice { Name = p.ProductName, Price = p.UnitPrice ?? 0 });

            return await this.GetFullResponse(query);
        }

        /// <summary>
        /// Gets a product report with largest category.
        /// </summary>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        public async Task<ProductReport<ProductPrice>> GetLargestCategoryProductsReport()
        {
            var response = await this.GetFullResponse(this.entities.Products);

            var result = response.Products
                .Where(p => p.UnitPrice != null)
                .GroupBy(p => p.Category)
                .OrderBy(c => c.Count())
                .First()
                .Select(p => new ProductPrice { Name = p.ProductName, Price = p.UnitPrice ?? 0 });

            return new ProductReport<ProductPrice>(result);
        }

        /// <summary>
        /// Gets a product report with most expensive category.
        /// </summary>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        public async Task<ProductReport<ProductPrice>> GetMostExpensiveCategoryProductsReport()
        {
            var response = await this.GetFullResponse(this.entities.Products);

            var result = response.Products
                .Where(p => p.UnitPrice != null)
                .GroupBy(p => p.Category)
                .OrderBy(c => c.Sum(p => p.UnitPrice))
                .First()
                .Select(p => new ProductPrice { Name = p.ProductName, Price = p.UnitPrice ?? 0 });

            return new ProductReport<ProductPrice>(result);
        }

        private async Task<ProductReport<T>> GetFullResponse<T>(DataServiceQuery<T> query)
        {
            var result = new ProductReport<T>();
            DataServiceQueryContinuation<T> token = null;

            var response = await Task<IEnumerable<T>>.Factory
                .FromAsync(query.BeginExecute(null, null), (ar) => query.EndExecute(ar))
                as QueryOperationResponse<T>;

            do
            {
                if (token != null)
                {
                    response = await Task<IEnumerable<T>>.Factory
                        .FromAsync(this.entities.BeginExecute<T>(token, null, null), (ar) => this.entities.EndExecute<T>(ar))
                        as QueryOperationResponse<T>;
                }

                foreach (var product in response)
                {
                    result.Products.Add(product);
                }
            }
            while ((token = response.GetContinuation()) != null);

            return result;
        }
    }
}
