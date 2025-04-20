namespace InventoryAPI.Application.Products.Command.Create;

public record CreateProductCommand(string Name, string Description, string? Sku, decimal? Price);
