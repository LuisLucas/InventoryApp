using HateoasGenerator.Helpers;
using Microsoft.CodeAnalysis;

namespace HateoasGenerator.Interfaces;
internal static class IHateoas
{
    private const string FileName = "IHateoas";
    public const string Class = @"
using HateoasLib.Models.ResponseModels;
using HateoasLib.Models;

namespace HateoasLib.Interfaces;

public interface IHateoas<T, R>
{
    CollectionResource<R> CreateCollectionResponse(IEnumerable<R> items,
        List<ControllerAction> listActions,
        List<ControllerAction<R, object>> itemActions);
}";

    internal static IncrementalGeneratorInitializationContext AddIHateoasToSource(this IncrementalGeneratorInitializationContext context)
    {
        return context.AddFileToSource(Class, FileName);
    }
}
