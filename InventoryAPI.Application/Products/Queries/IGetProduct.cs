namespace InventoryAPI.Application.Products.Queries
{
    public interface IGetProduct
    {
        Task<IEnumerable<ProductDto>> GetProducts();
        Task<ProductDto> GetProduct(int id);
    }
}
