namespace InventoryAPI.Application.Products.Common {
    public static class ProductMapper {
        public static ProductDto MapFromProduct(Domain.Entities.Product product) {
            return new ProductDto(
                                product.Id,
                                product.Name ?? string.Empty,
                                product.Description ?? string.Empty,
                                product.Sku,
                                product.Price,
                                product.CreatedAt,
                                product.CreatedBy,
                                product.LastUpdatedAt,
                                product.LastUpdatedBy
                                );
        }
    }
}
