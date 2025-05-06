using System.Reflection;
using HateoasLib.Hateoas;
using HateoasLib.Models;
using HateoasLib.Models.ResponseModels;
using HateoasLib.Attributes;
using InventoryApi.Controllers;
using InventoryAPI.Application.Products;

namespace InventoryApi.Models;

[Hateoas("Get", "self", "Id")]
[Hateoas("Put", "update", "Id")]
[Hateoas("Delete", "delete", "Id")]
public record ProductModel(int Id, string Name, string Description, string? Sku, decimal? Price, DateTime? CreatedAt, string? CreatedBy, DateTime? LastUpdatedAt, string? LastUpdatedBy);
