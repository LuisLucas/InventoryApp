
using InventoryAPI.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Application.Products.Queries
{
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
            return products.Select(prod => 
                    new ProductDto( 
                                prod.Id, 
                                prod.Name, 
                                prod.Description, 
                                prod.Sku, 
                                prod.Price, 
                                prod.CreatedAt, 
                                prod.CreatedBy, 
                                prod.LastUpdatedAt, 
                                prod.LastUpdatedBy
                                ));
        }
    }
}
