using HateoasGenerator.Helpers;
using Microsoft.CodeAnalysis;

namespace HateoasGenerator.Models;
internal static class ControllerActionRecords
{
    private const string FileName = "ControllerActionRecords";
    public const string Class = @"
namespace HateoasLib.Models;

public record ControllerAction(string action, object? values, string rel, string method);

public record ControllerAction<T, R>(string action, Tuple<string, Func<T, R>> values, string rel, string method);";

    internal static IncrementalGeneratorInitializationContext AddControllerActionToSource(this IncrementalGeneratorInitializationContext context)
    {
        return context.AddFileToSource(Class, FileName);
    }
}
