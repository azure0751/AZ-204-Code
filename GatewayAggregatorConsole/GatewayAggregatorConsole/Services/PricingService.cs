using System;
using System.Diagnostics;
using System.Threading.Tasks;

public class PricingService : IPricingService
{
    public async Task<Price> GetPriceByProductIdAsync(Guid productId)
    {
        // Simulate async operation
        await Task.Delay(100);
        return new Price
        {
            ProductId = productId,
            Amount = 29.99m,
            Currency = "USD"
        };
    }
}
