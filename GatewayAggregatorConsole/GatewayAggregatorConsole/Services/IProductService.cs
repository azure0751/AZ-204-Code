using System;
using System.Threading.Tasks;

public interface IProductService
{
    Task<Product> GetProductByIdAsync(Guid productId);
}
