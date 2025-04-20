
namespace InventoryAPI.Application.Stocks.Command;

public interface IUpdateStock
{
    Task<ProductStockDto?> Handle(UpdateStockCommand command);
}
