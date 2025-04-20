using InventoryAPI.Application.Common;
using InventoryAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Application.Stocks.Command {
    public class UpdateStockCommandHandler(IDbContext context) : IUpdateStock {
        public async Task<ProductStockDto?> Handle(UpdateStockCommand command) {
            var productId = command.ProductId;
            if (productId <= 0) {
                return default;
            }

            var product = context.Products.Find(productId);
            if (product == null) {
                return default;
            }

            var stock = await context.Stocks.SingleOrDefaultAsync(x => x.ProductId == productId);
            if (stock == null) {
                if (command.ChangeInStock <= 0) {
                    return default;
                }

                return await CreateAndReturnProductStock(command, productId);
            }

            var newStock = stock.Quantity - command.ChangeInStock;
            if (newStock < 0) {
                return default;
            }

            stock.Quantity = newStock;
            await context.SaveChangesAsync(new CancellationToken());
            return new ProductStockDto(stock.Quantity.Value, productId);
        }

        private async Task<ProductStockDto?> CreateAndReturnProductStock(UpdateStockCommand command, int productId) {
            var stock = new Stock() {
                ProductId = productId,
                Quantity = command.ChangeInStock,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Admin",
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = "Admin"
            };
            context.Stocks.Add(stock);
            await context.SaveChangesAsync(new CancellationToken());
            return new ProductStockDto(stock.Quantity.Value, productId);
        }
    }
}
