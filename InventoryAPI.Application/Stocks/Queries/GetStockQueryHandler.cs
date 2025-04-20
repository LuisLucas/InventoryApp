using InventoryAPI.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Application.Stocks.Queries;

public class GetStockQueryHandler(IDbContext context) : IGetStock
{
    public async Task<ProductStockDto?> Handle(int productId)
    {
        if (productId <= 0)
        {
            return default;
        }

        Domain.Entities.Product? product = context.Products.Find(productId);
        if (product == null)
        {
            return default;
        }

        Domain.Entities.Stock? stock = await context.Stocks.SingleOrDefaultAsync(x => x.ProductId == productId);
        if (stock == null)
        {
            return default;
        }

        return new ProductStockDto(stock.Quantity ?? 0, productId);
    }
}
