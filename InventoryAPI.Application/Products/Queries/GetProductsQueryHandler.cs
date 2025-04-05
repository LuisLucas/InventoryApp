using InventoryAPI.Application.Common;
using InventoryAPI.Application.Products.Common;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Application.Products.Queries {
    public class GetProductsQueryHandler : IGetProducts {
        private readonly IDbContext _dbContext;

        public GetProductsQueryHandler(IDbContext context) 
        {
            this._dbContext = context;
        }


        public async Task<IEnumerable<ProductDto>> Handle() {
            var products = await this._dbContext.Products.ToListAsync();
            return products.Select(product => ProductMapper.MapFromProduct(product));
        }

        public async Task<ProductDto> Handle(int productId) {
            var product = await this._dbContext.Products.FindAsync(productId);
            if (product == null) {
                return null;
            }

            return ProductMapper.MapFromProduct(product);
        }
    }
}
