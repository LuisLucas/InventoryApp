using HateoasGenerator.Helpers;
using Microsoft.CodeAnalysis;

namespace HateoasGenerator.HateoasFactory;
internal static class HateoasFactory
{
    private const string FileName = "HateoasFactory";
    private const string Class = @"
using HateoasLib.Models;
using HateoasLib.Models.ResponseModels;
using HateoasLib.Hateoas;
using HateoasLib.Interfaces;

namespace HateoasLib.Hateoas;

public class HateoasFactory(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor) : IHateoasFactory
{
    public Resource<T> CreateResponse<T, R>(
                        string controller,
                        T item,
                        List<ControllerAction<T, R>> itemActions)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);

        (string? scheme, HostString? host) = ExtractSchemeAndHost();

        var resourceItes = new Resource<T>();
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
            Links = BuildLinks(linkGenerator, controller, scheme, host.Value, itemControllerActions)
        };
        return resource;
    }

    public Resource<T> CreateResponse<T>(
                                string controller,
                                T item,
                                List<ControllerAction> itemActions)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);

        (string? scheme, HostString? host) = ExtractSchemeAndHost();
        var resource = new Resource<T>
        {
            Item = item,
            Links = BuildLinks(linkGenerator, controller, scheme, host.Value, itemActions)
        };
        return resource;
    }

    public CollectionResource<T> CreateCollectionResponse<T, R>(
        string controller,
        IEnumerable<T> items,
        List<ControllerAction> listActions,
        List<ControllerAction<T, R>> itemActions)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);

        (string? scheme, HostString? host) = ExtractSchemeAndHost();

        var collectionResponse = new CollectionResource<T>
        {
            Items = AddLinkstoItems(linkGenerator, controller, scheme, host.Value, items, itemActions),
            Links = BuildLinks(linkGenerator, controller, scheme, host.Value, listActions)
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
            var resource = AddLinksToItem(linkGenerator, controller, scheme, host, item, itemActions);
            resourceItems.Add(resource);
        }
        return resourceItems;
    }

    private static Resource<T> AddLinksToItem<T, R>(LinkGenerator linkGenerator,
        string controller,
        string scheme,
        HostString host,
        T item,
        List<ControllerAction<T, R>> itemActions)
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
        return resource;
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

    private (string?, HostString?) ExtractSchemeAndHost()
    {
        var request = httpContextAccessor.HttpContext?.Request;
        return (request?.Scheme, request?.Host);
    }
}";

    internal static IncrementalGeneratorInitializationContext AddFactoryToSource(this IncrementalGeneratorInitializationContext context)
    {
        return context.AddFileToSource(Class, FileName);
    }
}
