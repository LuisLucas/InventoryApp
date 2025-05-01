using HateoasGenerator.Helpers;
using Microsoft.CodeAnalysis;

namespace HateoasGenerator.Models;
internal static class Link
{
    private const string FileName = "Link";
    public const string Class = @"
namespace HateoasLib.Models.ResponseModels;
public record class Link(string Href, string Rel, string Method);";

    internal static IncrementalGeneratorInitializationContext AddLinkToSource(this IncrementalGeneratorInitializationContext context)
    {
        return context.AddFileToSource(Class, FileName);
    }
}
