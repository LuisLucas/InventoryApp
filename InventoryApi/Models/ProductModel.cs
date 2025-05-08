using HateoasLib.Attributes;

namespace InventoryApi.Models;

[Hateoas("Get", "self", "Id")]
[Hateoas("Put", "update", "Id")]
[Hateoas("Delete", "delete", "Id")]
public record ProductModel(int Id, string Name, string Description, string? Sku, decimal? Price, DateTime? CreatedAt, string? CreatedBy, DateTime? LastUpdatedAt, string? LastUpdatedBy);
