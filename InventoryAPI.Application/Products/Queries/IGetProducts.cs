namespace InventoryAPI.Application.Products.Queries {
    public interface IGetProducts {
        Task<IEnumerable<ProductDto>> Handle();

        Task<ProductDto> Handle(int productId);
    }
}
