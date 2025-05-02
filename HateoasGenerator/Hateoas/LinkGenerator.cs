using HateoasGenerator.Helpers;
using Microsoft.CodeAnalysis;

namespace HateoasGenerator.Hateoas;
internal static class LinkGenerator
{
    private const string FileName = "GenerateLinks";
    private const string Class = @"
using HateoasLib.Models.ResponseModels;
using HateoasLib.Models;

namespace HateoasLib.Hateoas;

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
}";

    internal static IncrementalGeneratorInitializationContext AddGenerateLinksToSource(this IncrementalGeneratorInitializationContext context)
    {
        return context.AddFileToSource(Class, FileName);
    }
}
