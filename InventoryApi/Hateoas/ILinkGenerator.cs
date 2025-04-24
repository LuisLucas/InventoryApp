using InventoryAPI.Application.Common;

namespace InventoryAPI.Hateoas;
public interface ILinkGenerator
{
    List<Link> BuildLinks(string controller, IEnumerable<ControllerAction> actions, string scheme, HostString host);
}

public record ControllerAction(string action, object? values, string rel, string method);
