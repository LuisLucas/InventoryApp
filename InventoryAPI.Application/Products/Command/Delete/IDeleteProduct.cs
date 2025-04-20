namespace InventoryAPI.Application.Products.Command.Delete;

public interface IDeleteProduct
{
    Task<bool> Handle(DeleteProductCommand request);
}
