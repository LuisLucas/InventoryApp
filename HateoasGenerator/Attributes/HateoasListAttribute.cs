using HateoasGenerator.Helpers;
using Microsoft.CodeAnalysis;

namespace HateoasGenerator.Attributes;
internal static class HateoasListAttribute
{
    internal const string FileName = "HateoasListAttribute";
    private const string Class = @"
namespace HateoasLib.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class HateoasListAttribute : Attribute
{
    public string Method { get; }
    public string Relation { get; }
    public string Property { get; }

    public HateoasListAttribute(string method, string relation, string property)
    {
        Method = method;
        Relation = relation;
        Property = property;
    }
}";

    internal static IncrementalGeneratorInitializationContext AddHateoasListAttributeToSource(this IncrementalGeneratorInitializationContext context)
    {
        return context.AddFileToSource(Class, FileName);
    }
}
