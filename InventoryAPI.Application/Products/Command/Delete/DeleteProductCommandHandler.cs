using InventoryAPI.Application.Common;

namespace InventoryAPI.Application.Products.Command.Delete
{
    public class DeleteProductCommandHandler : IDeleteProduct 
    {
        private IDbContext _dbContext;

        public DeleteProductCommandHandler(IDbContext dbContext) 
        {
            this._dbContext = dbContext;
        }
        public async Task<bool> Handle(DeleteProductCommand request) 
        {
            var productToDelete = await this._dbContext.Products.FindAsync(request.Id);
            if (productToDelete == null) {
                return false;
            }

            this._dbContext.Products.Remove(productToDelete);
            await this._dbContext.SaveChangesAsync(new CancellationToken());
            return true;
        }
    }
}
