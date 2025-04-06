namespace InventoryAPI.Application.Products.Command.Create {
    public class CreateProductCommand {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Sku { get; set; }

        public decimal? Price { get; set; }
    }
}
