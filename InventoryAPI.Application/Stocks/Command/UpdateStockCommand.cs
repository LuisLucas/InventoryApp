namespace InventoryAPI.Application.Stocks.Command;

public record UpdateStockCommand(int ProductId, int ChangeInStock);
