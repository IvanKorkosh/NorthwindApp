using System;
using System.Threading.Tasks;
using Northwind.CurrencyServices.CountryCurrency;
using Northwind.CurrencyServices.CurrencyExchange;
using Northwind.ReportingServices.OData.ProductReports;
using Northwind.ReportingServices.ProductReports;

#pragma warning disable S1075
#pragma warning disable S3776

namespace ReportingApp
{
    /// <summary>
    /// Program class.
    /// </summary>
    public static class Program
    {
        private const string NorthwindServiceUrl = "https://services.odata.org/V3/Northwind/Northwind.svc";
        private const string ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\USER\desktop\course3\northwind-apps-module-3\LocalDB\Northwind.mdf;Integrated Security=True;Connect Timeout=30";
        private const string AccessToken = "c73f1de173d3a3a35625358b5a42c23b";
        private const string CurrentProductsReport = "current-products";
        private const string MostExpensiveProductsReport = "most-expensive-products";
        private const string PriceLessThenProductsReport = "price-less-then-products";
        private const string PriceBetweenProductsReport = "price-between-products";
        private const string PriceAboveAverageProductsReport = "price-above-average-products";
        private const string UnitsInStockDeficitReport = "units-in-stock-deficit";
        private const string MostCheapProductsReport = "most-cheap-products";
        private const string LargestCategoryProductsReport = "largest-category-products";
        private const string MostExpensiveCategoryProductsReport = "most-expensive-category-products";
        private const string CurrentProductsLocalPricesReport = "current-products-local-prices";
        private static IProductReportService service;

        /// <summary>
        /// A program entry point.
        /// </summary>
        /// <param name="args">Program arguments.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task Main(string[] args)
        {
            if (args == null || args.Length < 1)
            {
                ShowHelp();
                return;
            }

            service = new Northwind.ReportingServices.SqlService.ProductReports.ProductReportService(ConnectionString);
            var reportName = args[0];

            if (string.Equals(reportName, CurrentProductsReport, StringComparison.InvariantCultureIgnoreCase))
            {
                await ShowCurrentProducts();
            }
            else if (string.Equals(reportName, MostExpensiveProductsReport, StringComparison.InvariantCultureIgnoreCase))
            {
                if (args.Length > 1 && int.TryParse(args[1], out int count))
                {
                    await ShowMostExpensiveProducts(count);
                }
            }
            else if (string.Equals(reportName, PriceLessThenProductsReport, StringComparison.InvariantCultureIgnoreCase))
            {
                if (args.Length > 1 && decimal.TryParse(args[1], out decimal price))
                {
                    await ShowPriceLessThenProducts(price);
                }
            }
            else if (string.Equals(reportName, PriceBetweenProductsReport, StringComparison.InvariantCultureIgnoreCase))
            {
                if (args.Length > 2 && decimal.TryParse(args[1], out decimal leftPrice) && decimal.TryParse(args[2], out decimal rightPrice))
                {
                    await ShowPriceBetweenProducts(leftPrice, rightPrice);
                }
            }
            else if (string.Equals(reportName, PriceAboveAverageProductsReport, StringComparison.InvariantCultureIgnoreCase))
            {
                await ShowPriceAboveAverageProducts();
            }
            else if (string.Equals(reportName, UnitsInStockDeficitReport, StringComparison.InvariantCultureIgnoreCase))
            {
                await ShowUnitsInStockDeficit();
            }
            else if (string.Equals(reportName, MostCheapProductsReport, StringComparison.InvariantCultureIgnoreCase))
            {
                if (args.Length > 1 && int.TryParse(args[1], out int count))
                {
                    await ShowMostCheapProducts(count);
                }
            }
            else if (string.Equals(reportName, LargestCategoryProductsReport, StringComparison.InvariantCultureIgnoreCase))
            {
                await ShowLargestCategoryProducts();
            }
            else if (string.Equals(reportName, MostExpensiveCategoryProductsReport, StringComparison.InvariantCultureIgnoreCase))
            {
                await ShowMostExpensiveCategoryProducts();
            }
            else if (string.Equals(reportName, CurrentProductsLocalPricesReport, StringComparison.InvariantCultureIgnoreCase))
            {
                await ShowCurrentProductsLocalPrices();
            }
            else
            {
                ShowHelp();
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("\tReportingApp.exe <report> <report-argument1> <report-argument2> ...");
            Console.WriteLine();
            Console.WriteLine("Reports:");
            Console.WriteLine($"\t{CurrentProductsReport}\t\tShows current products.");
            Console.WriteLine($"\t{MostExpensiveProductsReport}\t\tShows specified number of the most expensive products.");
        }

        private static async Task ShowCurrentProducts()
        {
            var report = await service.GetCurrentProductsReport();
            PrintProductReport("current products:", report);
        }

        private static async Task ShowMostExpensiveProducts(int count)
        {
            var report = await service.GetMostExpensiveProductsReport(count);
            PrintProductReport($"{count} most expensive products:", report);
        }

        private static async Task ShowPriceLessThenProducts(decimal price)
        {
            var report = await service.GetPriceLessThenProductsReport(price);
            PrintProductReport($"products with price less than {price}:", report);
        }

        private static async Task ShowPriceBetweenProducts(decimal leftPrice, decimal rightPrice)
        {
            var report = await service.GetPriceBetweenProductsReport(leftPrice, rightPrice);
            PrintProductReport($"Products with price between {leftPrice} and {rightPrice}:", report);
        }

        private static async Task ShowPriceAboveAverageProducts()
        {
            var report = await service.GetPriceAboveAverageProductsReport();
            PrintProductReport($"Products with price above average:", report);
        }

        private static async Task ShowUnitsInStockDeficit()
        {
            var report = await service.GetUnitsInStockDeficitReport();
            PrintProductReport($"Deficit products:", report);
        }

        private static async Task ShowMostCheapProducts(int count)
        {
            var report = await service.GetMostCheapProductsReport(count);
            PrintProductReport($"{count} most cheap products:", report);
        }

        private static async Task ShowLargestCategoryProducts()
        {
            var report = await service.GetLargestCategoryProductsReport();
            PrintProductReport($"Products in largest category:", report);
        }

        private static async Task ShowMostExpensiveCategoryProducts()
        {
            var report = await service.GetMostExpensiveCategoryProductsReport();
            PrintProductReport($"Products in most expensive category:", report);
        }

        private static async Task ShowCurrentProductsLocalPrices()
        {
            var reportPrinter = new CurrentProductLocalPriceReport(service, new CurrencyExchangeService(AccessToken), new CountryCurrencyService());
            await reportPrinter.PrintReport();
        }

        private static void PrintProductReport(string header, ProductReport<ProductPrice> productReport)
        {
            Console.WriteLine($"Report - {header}");
            foreach (var reportLine in productReport.Products)
            {
                Console.WriteLine("{0}, {1}", reportLine.Name, reportLine.Price);
            }
        }
    }
}
