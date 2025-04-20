namespace InventoryAPI.Domain.Entities;

public partial class Product
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public string? Sku { get; set; }

    public decimal? Price { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? LastUpdatedAt { get; set; }

    public string? LastUpdatedBy { get; set; }

    public virtual ICollection<Stock> Stocks { get; set; } = [];
}
