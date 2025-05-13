using HateoasGenerator.Helpers;
using Microsoft.CodeAnalysis;

namespace HateoasGenerator.Attributes;
internal static class HateoasAttribute
{
    internal const string FileName = "HateoasAttribute";
    private const string Class = @"
namespace HateoasLib.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class HateoasAttribute : Attribute
{
    public string Method { get; }
    public string Relation { get; }
    public string Property { get; }
    public string Controller { get; }

    public HateoasAttribute(string method, string relation, string property, string controller = """")
    {
        Method = method;
        Relation = relation;
        Property = property;
        Controller = controller;
    }
}";

    internal static IncrementalGeneratorInitializationContext AddHateoasAttributeToSource(this IncrementalGeneratorInitializationContext context)
    {
        return context.AddFileToSource(Class, FileName);
    }
}
