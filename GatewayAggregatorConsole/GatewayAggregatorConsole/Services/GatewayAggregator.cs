using System;
using System.Threading.Tasks;

public class GatewayAggregator
{
    private readonly IProductService _productService;
    private readonly IPricingService _pricingService;

    public GatewayAggregator(IProductService productService, IPricingService pricingService)
    {
        _productService = productService;
        _pricingService = pricingService;
    }

    public async Task<ProductPrice> GetProductPriceAsync(Guid productId)
    {
        var productTask = _productService.GetProductByIdAsync(productId);
        var priceTask = _pricingService.GetPriceByProductIdAsync(productId);

        await Task.WhenAll(productTask, priceTask);

        return new ProductPrice
        {
            Product = await productTask,
            Price = await priceTask
        };
    }
}
