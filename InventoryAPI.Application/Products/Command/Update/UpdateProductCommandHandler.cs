using InventoryAPI.Application.Common;
using InventoryAPI.Application.Products.Common;

namespace InventoryAPI.Application.Products.Command.Update;

public class UpdateProductCommandHandler(IDbContext dbContext) : IUpdateProduct
{

    public async Task<ProductDto> Handle(UpdateProductCommand request)
    {

        Domain.Entities.Product? productEntity = await dbContext.Products.FindAsync(request.Id);
        if (productEntity == null)
        {
            return null; // return exception
        }

        productEntity.Name = request.Name;
        productEntity.Description = request.Description;
        productEntity.Sku = request.Sku;
        productEntity.Price = request.Price;
        productEntity.LastUpdatedBy = "System";
        productEntity.LastUpdatedAt = DateTime.UtcNow;

        var cancelationToken = new CancellationToken();
        await dbContext.SaveChangesAsync(cancelationToken);

        // Return the product
        return ProductMapper.MapFromProduct(productEntity);
    }
}
