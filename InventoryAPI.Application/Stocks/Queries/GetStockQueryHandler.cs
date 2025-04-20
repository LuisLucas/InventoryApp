using InventoryAPI.Application.Common;
using System.Data.Entity;

namespace InventoryAPI.Application.Stocks.Queries {
    public class GetStockQueryHandler(IDbContext context) : IGetStock {
        public async Task<ProductStockDto?> Handle(int productId) {
            if (productId <= 0) {
                return default;
            }

            var product = context.Products.Find(productId);
            if (product == null) {
                return default;
            }

            var stock = await context.Stocks.SingleOrDefaultAsync(x => x.ProductId == productId);
            if (stock == null) {
                return default;
            }

            return new ProductStockDto(stock.Quantity ?? 0, productId);
        }
    }
}
