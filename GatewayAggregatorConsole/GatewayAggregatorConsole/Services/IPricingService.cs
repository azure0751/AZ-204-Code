using System;
using System.Diagnostics;
using System.Threading.Tasks;

public interface IPricingService
{
    Task<Price> GetPriceByProductIdAsync(Guid productId);
}
