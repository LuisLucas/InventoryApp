using InventoryAPI.Application.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAPI.Application.Stocks.Queries {
    public class GetStockQueryHandler(IDbContext context) : IGetStock {
        public async Task<int> Handle(int productId) {
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

            return stock.Quantity.Value;
        }
    }
}
