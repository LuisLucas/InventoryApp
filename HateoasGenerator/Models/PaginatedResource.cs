using HateoasGenerator.Helpers;
using Microsoft.CodeAnalysis;

namespace HateoasGenerator.Models;
internal static class PaginatedResource
{
    private const string FileName = "CollectionResource";
    public const string Class = @"
namespace HateoasLib.Models.ResponseModels;

public class PaginatedResource<T>
{
    public List<Resource<T>> Items { get; set; }

    public List<Link> Links { get; set; }

    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
}";

    internal static IncrementalGeneratorInitializationContext AddPaginatedResourceToSource(this IncrementalGeneratorInitializationContext context)
    {
        return context.AddFileToSource(Class, FileName);
    }
}
