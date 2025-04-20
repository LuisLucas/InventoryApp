using InventoryAPI.Application.Common;
using InventoryAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Application.Stocks.Command;

public class UpdateStockCommandHandler(IDbContext context) : IUpdateStock
{
    public async Task<ProductStockDto?> Handle(UpdateStockCommand command)
    {
        int productId = command.ProductId;
        if (productId <= 0)
        {
            return default;
        }

        Product? product = context.Products.Find(productId);
        if (product == null)
        {
            return default;
        }

        var productBusinessEntity = new Domain.Models.Product(product);
        Stock? stock = await context.Stocks.SingleOrDefaultAsync(x => x.ProductId == productId);
        if (stock == null)
        {
            stock = productBusinessEntity.CreateStock(command.ChangeInStock);
            context.Stocks.Add(stock);
        }
        else
        {
            productBusinessEntity.UpdateStock(stock, command.ChangeInStock);
        }

        await context.SaveChangesAsync(new CancellationToken());
        return new ProductStockDto(stock.Quantity.Value, productId);
    }
}
