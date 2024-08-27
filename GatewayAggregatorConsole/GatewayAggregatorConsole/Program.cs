using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Instantiate services
        var productService = new ProductService();
        var pricingService = new PricingService();
        var gatewayAggregator = new GatewayAggregator(productService, pricingService);

        // Example product ID
        var productId = Guid.NewGuid();

        // Get product and pricing information
        var productPrice = await gatewayAggregator.GetProductPriceAsync(productId);

        // Display results
        Console.WriteLine($"Product Name: {productPrice.Product.Name}");
        Console.WriteLine($"Description: {productPrice.Product.Description}");
        Console.WriteLine($"Price: {productPrice.Price.Amount} {productPrice.Price.Currency}");
    }
}
