namespace InventoryAPI.Application.Stocks.Queries {
    public interface IGetStock {
        Task<int> Handle(int productId);
    }
}
