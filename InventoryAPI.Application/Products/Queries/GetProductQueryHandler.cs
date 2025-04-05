using InventoryAPI.Application.Common;

namespace InventoryAPI.Application.Products.Queries {
    public class GetProductQueryHandler : IGetProduct {
        private readonly IDbContext _dbContext;

        public GetProductQueryHandler(IDbContext context) 
        {
            this._dbContext = context;
        }


        public async Task<ProductDto> Handle(int productId) {
            var product = await this._dbContext.Products.FindAsync(productId);
            if (product == null) {
                return null;
            }

            return ProductMapper.MapFromProduct(product);
        }
    }

    public interface IGetProduct {
        Task<ProductDto> Handle(int productId);
    }

}
