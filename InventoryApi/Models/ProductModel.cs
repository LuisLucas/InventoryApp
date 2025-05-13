using HateoasLib.Attributes;

namespace InventoryApi.Models;

[Hateoas("GET", "self", "Id")]
[Hateoas("PUT", "edit", "Id")]
[Hateoas("DELETE", "delete", "Id")]
[Hateoas("GET", "related", "Id", "stocks")]
[HateoasList("GET", "self")]
[HateoasList("POST", "create")]
public record ProductModel(int Id, string Name, string Description, string? Sku, decimal? Price, DateTime? CreatedAt, string? CreatedBy, DateTime? LastUpdatedAt, string? LastUpdatedBy);
