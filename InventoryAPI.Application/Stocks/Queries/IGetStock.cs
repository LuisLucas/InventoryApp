namespace InventoryAPI.Application.Stocks.Queries;

public interface IGetStock
{
    Task<ProductStockDto?> Handle(int productId);
}
