namespace InventoryAPI.Application.Products.Command.Update
{
    public record class UpdateProductCommand(int Id, string? Name, string? Description, string? Sku, decimal? Price);
}
