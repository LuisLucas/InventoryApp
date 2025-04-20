namespace InventoryAPI.Application.Products.Command.Create;

public interface ICreateProduct
{
    Task<ProductDto> Handle(CreateProductCommand request);
}
