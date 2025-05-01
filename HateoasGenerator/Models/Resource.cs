using HateoasGenerator.Helpers;
using Microsoft.CodeAnalysis;

namespace HateoasGenerator.Models;
internal static class Resource
{
    private const string FileName = "Resource";
    public const string Class = @"
namespace HateoasLib.Models.ResponseModels;

public class Resource<T>
{
    public T Item { get; set; }

    public List<Link> Links { get; set; }
}";

    internal static IncrementalGeneratorInitializationContext AddResourceToSource(this IncrementalGeneratorInitializationContext context)
    {
        return context.AddFileToSource(Class, FileName);
    }
}
