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

namespace HateoasLib.Hateoas;

public class HateoasFactory(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor) : IHateoasFactory
{
    public void CreateResponse<T>()
    {
        throw new NotImplementedException();
    }

    public CollectionResource<T> CreateCollectionResponse<T, R>(
        string controller,
        IEnumerable<T> items,
        List<ControllerAction> listActions,
        List<ControllerAction<T, R>> itemActions)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);

        string scheme = httpContextAccessor.HttpContext.Request.Scheme;
        HostString host = httpContextAccessor.HttpContext.Request.Host;

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
}";

    internal static IncrementalGeneratorInitializationContext AddFactoryToSource(this IncrementalGeneratorInitializationContext context)
    {
        return context.AddFileToSource(Class, FileName);
    }
}
