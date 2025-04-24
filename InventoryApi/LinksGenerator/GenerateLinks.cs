using InventoryAPI.Application.Common;
using InventoryAPI.LinksGenerator;

namespace InventoryApi.LinksGenerator;

public static class GenerateLinks
{
    public static List<Link> BuildLinks(LinkGenerator linkGenerator, string controller, IEnumerable<ControllerAction> actions, string scheme, HostString host)
    {
        var links = new List<Link>();
        foreach (ControllerAction item in actions)
        {
            string? absoluteUri = linkGenerator.GetUriByAction(
                                                action: item.action,
                                                controller: controller,
                                                values: item.values,
                                                scheme,
                                                host);

            if (!string.IsNullOrEmpty(absoluteUri))
            {
                links.Add(new Link(absoluteUri, item.rel, item.method));
            }
        }
        return links;
    }
}
