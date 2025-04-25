using InventoryAPI.Application.Products;
using InventoryAPI.Hateoas;

namespace InventoryApi.Hateoas;

public static class HateoasFactory
{
    public static void CreateResponse<T>(IEnumerable<ProductDto> products)
    {
        throw new NotImplementedException();
    }

    public static CollectionResource<T> CreateCollectionResponse<T, R>(
        LinkGenerator linkGenerator,
        string controller,
        string scheme,
        HostString host,
        IEnumerable<T> items,
        List<ControllerAction> listActions,
        List<ControllerAction<T, R>> itemActions)
    {
        var collectionResponse = new CollectionResource<T>
        {
            Items = AddLinkstoItems(linkGenerator, controller, scheme, host, items, itemActions),
            Links = BuildLinks(linkGenerator, controller, scheme, host, listActions)
        };
        return collectionResponse;
    }

    private static List<Resource<T>> AddLinkstoItems<T, R>(
        LinkGenerator linkGenerator,
        string controller,
        string scheme,
        HostString host,
        IEnumerable<T> items,
        List<ControllerAction<T, R>> itemActions)
    {
        var resourceItems = new List<Resource<T>>();
        foreach (T? item in items)
        {
            var itemControllerActions = new List<ControllerAction>();
            foreach (ControllerAction<T, R> c in itemActions)
            {
                var routeValueDic = new RouteValueDictionary
                {
                    { c.values.Item1, c.values.Item2.Invoke(item) }
                };
                itemControllerActions.Add(new ControllerAction(c.action, routeValueDic, c.rel, c.method));
            }

            var resource = new Resource<T>
            {
                Item = item,
                Links = BuildLinks(linkGenerator, controller, scheme, host, itemControllerActions)
            };
            resourceItems.Add(resource);
        }
        return resourceItems;
    }

    private static List<Link> BuildLinks(LinkGenerator linkGenerator, string controller, string scheme, HostString host, List<ControllerAction> listActions)
    {
        return GenerateLinks.BuildLinks(
                                    linkGenerator,
                                    controller,
                                    listActions,
                                    scheme,
                                    host);
    }
}
