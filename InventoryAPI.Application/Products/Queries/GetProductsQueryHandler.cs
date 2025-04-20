using InventoryAPI.Application.Common;
using InventoryAPI.Application.Products.Common;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Application.Products.Queries;

public class GetProductsQueryHandler(IDbContext dbContext) : IGetProducts
{
    public async Task<IEnumerable<ProductDto>> Handle()
    {
        List<Domain.Entities.Product> products = await dbContext.Products.ToListAsync();
        return products.Select(product => ProductMapper.MapFromProduct(product));
    }

    public async Task<ProductDto> Handle(int productId)
    {
        Domain.Entities.Product? product = await dbContext.Products.FindAsync(productId);
        if (product == null)
        {
            return null;
        }

        return ProductMapper.MapFromProduct(product);
    }
}
