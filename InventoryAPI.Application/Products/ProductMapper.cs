namespace InventoryAPI.Application.Products {
    public static class ProductMapper {
        public static ProductDto MapFromProduct(Domain.Entities.Product product) {
            return new ProductDto(
                                product.Id,
                                product.Name,
                                product.Description,
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
