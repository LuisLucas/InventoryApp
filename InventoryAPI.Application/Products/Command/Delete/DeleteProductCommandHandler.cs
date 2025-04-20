using InventoryAPI.Application.Common;

namespace InventoryAPI.Application.Products.Command.Delete;

public class DeleteProductCommandHandler(IDbContext dbContext) : IDeleteProduct
{
    public async Task<bool> Handle(DeleteProductCommand request)
    {
        Domain.Entities.Product? productToDelete = await dbContext.Products.FindAsync(request.Id);
        if (productToDelete == null)
        {
            return false;
        }

        dbContext.Products.Remove(productToDelete);
        await dbContext.SaveChangesAsync(new CancellationToken());
        return true;
    }
}
