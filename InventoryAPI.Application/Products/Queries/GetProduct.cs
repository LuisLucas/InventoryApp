
using InventoryAPI.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Application.Products.Queries {
    public class GetProduct : IGetProduct
    {
        private readonly IDbContext _dbContext;

        public GetProduct(IDbContext context) 
        { 
            this._dbContext = context;
        }

        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            var products = await this._dbContext.Products.ToListAsync();           
            return products.Select(product => this.MapFromProduct(product));
        }

        async Task<ProductDto?> IGetProduct.GetProduct(int id) {
            var product = await this._dbContext.Products.FindAsync(id);
            if (product == null) {
                return null;
            }

            return this.MapFromProduct(product);
        }

        private ProductDto MapFromProduct(Domain.Entities.Product product) {
            return new ProductDto(
                                product.Id,
                                product.Name,
                                product.Description,
                                product.Sku,
                                product.Price,
                                product.CreatedAt,
                                product.CreatedBy,
                                product.LastUpdatedAt,
                                product.LastUpdatedBy
                                );
        }
    }
}
