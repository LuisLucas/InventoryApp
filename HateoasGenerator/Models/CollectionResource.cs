using HateoasGenerator.Helpers;
using Microsoft.CodeAnalysis;

namespace HateoasGenerator.Models;

internal static class CollectionResource
{
    private const string FileName = "CollectionResource";
    public const string Class = @"
namespace HateoasLib.Models.ResponseModels;

public class CollectionResource<T>
{
    public List<Resource<T>> Items { get; set; }

    public List<Link> Links { get; set; }
}";

    internal static IncrementalGeneratorInitializationContext AddCollectionResourceToSource(this IncrementalGeneratorInitializationContext context)
    {
        return context.AddFileToSource(Class, FileName);
    }
}
