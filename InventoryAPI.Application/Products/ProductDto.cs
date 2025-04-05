namespace InventoryAPI.Application.Products {
    public record ProductDto(int Id, string Name, string Description, string? Sku, decimal? Price, DateTime? CreatedAt, string? CreatedBy, DateTime? LastUpdatedAt, string? LastUpdatedBy);
}
