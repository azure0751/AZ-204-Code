using System;
using System.Threading.Tasks;

public class ProductService : IProductService
{
    public async Task<Product> GetProductByIdAsync(Guid productId)
    {
        // Simulate async operation
        await Task.Delay(100);
        return new Product
        {
            Id = productId,
            Name = "Sample Product",
            Description = "A sample product description."
        };
    }
}
