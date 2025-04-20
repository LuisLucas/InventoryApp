using InventoryAPI.Application.Common;
using InventoryAPI.Application.Products.Common;

namespace InventoryAPI.Application.Products.Command.Create;

public class CreateProductCommandHandler(IDbContext dbContext) : ICreateProduct
{
    public async Task<ProductDto> Handle(CreateProductCommand request)
    {
        var product = new Domain.Entities.Product
        {
            Name = request.Name,
            Description = request.Description,
            Sku = request.Sku,
            Price = request.Price,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System", // Replace by user
            LastUpdatedAt = DateTime.UtcNow,
            LastUpdatedBy = "System" // Replace by user
        };

        // Save the product using the repository
        await dbContext.Products.AddAsync(product);

        var cancelationToken = new CancellationToken();
        await dbContext.SaveChangesAsync(cancelationToken);

        // Return the product
        return ProductMapper.MapFromProduct(product);
    }
}
