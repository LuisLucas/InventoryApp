using HateoasGenerator.Helpers;
using Microsoft.CodeAnalysis;

namespace HateoasGenerator.Interfaces;
internal static class IHateoasFactory
{
    private const string FileName = "IHateoasFactory";
    private const string Class = @"
using HateoasLib.Models.ResponseModels;
using HateoasLib.Models;

namespace HateoasLib.Hateoas;

public interface IHateoasFactory
{
   CollectionResource<T> CreateCollectionResponse<T, R>(
                                                        string controller,
                                                        IEnumerable<T> items,
                                                        List<ControllerAction> listActions,
                                                        List<ControllerAction<T, R>> itemActions);
}";

    internal static IncrementalGeneratorInitializationContext AddIHateoasFactoryToSource(this IncrementalGeneratorInitializationContext context)
    {
        return context.AddFileToSource(Class, FileName);
    }
}
