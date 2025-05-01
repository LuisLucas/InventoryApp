using HateoasGenerator.Helpers;
using Microsoft.CodeAnalysis;

namespace HateoasGenerator.Interfaces;
internal static class IHateoas
{
    private const string FileName = "IHateoas";
    public const string InterfaceClass = @"
using HateoasLib.Models.ResponseModels;
using HateoasLib.Models;

namespace HateoasLib.Interfaces;

public interface IHateoas<T, R>
{
    CollectionResource<R> CreateCollectionResponse(IEnumerable<R> items,
        List<ControllerAction> listActions,
        List<ControllerAction<R, object>> itemActions);
}";

    internal static IncrementalGeneratorInitializationContext AddAttributeClassToSource(this IncrementalGeneratorInitializationContext context)
    {
        return context.AddFileToSource(InterfaceClass, FileName);
    }
}
