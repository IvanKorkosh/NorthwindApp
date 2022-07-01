using System;
using System.Threading.Tasks;
using Northwind.CurrencyServices.CountryCurrency;
using Northwind.CurrencyServices.CurrencyExchange;
using Northwind.ReportingServices.ProductReports;

namespace ReportingApp
{
    /// <summary>
    /// Represents a class to print product reports.
    /// </summary>
    public class CurrentProductLocalPriceReport
    {
        private readonly IProductReportService productReportService;
        private readonly ICurrencyExchangeService currencyExchangeService;
        private readonly ICountryCurrencyService countryCurrencyService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentProductLocalPriceReport"/> class.
        /// </summary>
        /// <param name="productReportService">Service to provide product reports.</param>
        /// <param name="currencyExchangeService">Service to provide currency exchange.</param>
        /// <param name="countryCurrencyService">Service to provide info about country.</param>
        public CurrentProductLocalPriceReport(IProductReportService productReportService, ICurrencyExchangeService currencyExchangeService, ICountryCurrencyService countryCurrencyService)
        {
            this.productReportService = productReportService ?? throw new ArgumentNullException(nameof(productReportService));
            this.currencyExchangeService = currencyExchangeService ?? throw new ArgumentNullException(nameof(currencyExchangeService));
            this.countryCurrencyService = countryCurrencyService ?? throw new ArgumentNullException(nameof(countryCurrencyService));
        }

        /// <summary>
        /// Print product report.
        /// </summary>
        /// /// <returns>Returns <see cref="Task"/>.</returns>
        public async Task PrintReport()
        {
            var report = await this.productReportService.GetCurrentProductsWithLocalCurrencyReport(this.countryCurrencyService, this.currencyExchangeService);

            Console.WriteLine("Report - current products with local price:");
            foreach (var product in report.Products)
            {
                Console.WriteLine($"{product.Name}, {product.Price}$, {product.Country}, {product.LocalPrice}{product.CurrencySymbol}");
            }
        }
    }
}
