using System.Drawing.Printing;
using InventoryAPI.Application.Common;
using InventoryAPI.Application.Products.Common;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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

        var productBusinessEntity = new Domain.Models.Product(productEntity);
        productBusinessEntity.UpdateProduct(request.Name, request.Description, request.Sku, request.Price);

        var cancelationToken = new CancellationToken();
        await dbContext.SaveChangesAsync(cancelationToken);

        // Return the product
        return ProductMapper.MapFromProduct(productEntity);
    }
}
