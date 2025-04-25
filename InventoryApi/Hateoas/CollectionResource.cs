using System.Resources;
using InventoryAPI.Hateoas;

namespace InventoryApi.Hateoas;

public class CollectionResource<T>
{
    public List<Resource<T>> Items { get; set; }

    public List<Link> Links { get; set; }
}
