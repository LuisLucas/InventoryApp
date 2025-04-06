namespace InventoryAPI.Application.Products.Command.Update
{
    public interface IUpdateProduct
    {
        Task<ProductDto> Handle(UpdateProductCommand request);
    }
}
