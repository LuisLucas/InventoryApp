using InventoryAPI.Application.Common;
using InventoryAPI.Domain.Entities;

namespace InventoryAPI.Application.Products.Command {
    public class CreateProduct : ICreateProduct 
    {
        private readonly IDbContext _dbContext;

        public CreateProduct(IDbContext context) {
            this._dbContext = context;
        }

        public async Task<int> Handle(CreateProductCommand request) {
            var product = new Product {
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
            await this._dbContext.Products.AddAsync(product);

            var cancelationToken = new CancellationToken();
            var productId = await this._dbContext.SaveChangesAsync(cancelationToken);

            // Return the product ID
            return product.Id;
        }
    }
}
