using HateoasGenerator.Helpers;
using Microsoft.CodeAnalysis;

namespace HateoasGenerator.Interfaces;
internal static class IHateoasOneArg
{
    private const string FileName = "IHateoasOneArg";
    public const string Class = @"
using HateoasLib.Models.ResponseModels;
using HateoasLib.Models;

namespace HateoasLib.Interfaces;

public interface IHateoas<T>
{
    Resource<T> CreateResponse(T item, Type controller);
}";

    internal static IncrementalGeneratorInitializationContext AddIHateoasOneArgToSource(this IncrementalGeneratorInitializationContext context)
    {
        return context.AddFileToSource(Class, FileName);
    }
}
