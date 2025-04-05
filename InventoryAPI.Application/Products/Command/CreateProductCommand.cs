﻿namespace InventoryAPI.Application.Products.Command {
    public class CreateProductCommand {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Sku { get; set; }

        public decimal? Price { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? LastUpdatedAt { get; set; }

        public string? LastUpdatedBy { get; set; }
    }
}
