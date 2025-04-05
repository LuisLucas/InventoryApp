using InventoryAPI.Application.Common;
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
    }

    public interface IGetProducts 
    {
        Task<IEnumerable<ProductDto>> Handle();
    }

}
