using HateoasGenerator.Helpers;
using Microsoft.CodeAnalysis;

namespace HateoasGenerator.Attributes;
internal static class EnableHateoasAttribute
{
    internal const string FileName = "EnableHateoasAttribute";
    private const string Class = @"
namespace HateoasLib.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class EnableHateoasAttribute : System.Attribute
    {
            public EnableHateoasAttribute(Type dtoType)
            {
                DtoType = dtoType;
            }

            public Type DtoType { get; }
    }
}";

    internal static IncrementalGeneratorInitializationContext AddAttributeToSource(this IncrementalGeneratorInitializationContext context)
    {
        return context.AddFileToSource(Class, FileName);
    }
}
