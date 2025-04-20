
namespace InventoryAPI.Application.Stocks.Command {
    public interface ICreateStock {
        Task<ProductStockDto> Handle(CreateStockCommand command);
    }
}
