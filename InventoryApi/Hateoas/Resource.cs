using InventoryAPI.Hateoas;

namespace InventoryApi.Hateoas;

public class Resource<T>
{
    public T Item { get; set; }

    public List<Link> Links { get; set; }
}
