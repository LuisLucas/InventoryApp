using System.Text;
using HateoasGenerator.Attributes;
using HateoasGenerator.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace HateoasGenerator;
public static class HateoasControllerClassGenerator
{
    internal static void GenerateHateoasControllerClass(SourceProductionContext spc, IEnumerable<INamedTypeSymbol> candidateClasses, Dictionary<string, string> iocRegistration, List<string> usingsForIOC)
    {
        if (candidateClasses == null || !candidateClasses.Any())
        {
            return;
        }

        IEnumerable<INamedTypeSymbol> controllerClasses = Get(candidateClasses);

        foreach (INamedTypeSymbol symbol in controllerClasses)
        {
            (string dtoName, string dtoNamespace) = GetTypeFromAttribute(symbol);
            if (string.IsNullOrEmpty(dtoName) || string.IsNullOrEmpty(dtoNamespace))
            {
                continue;
            }

            AddControllerHateoasClassToSource(spc, symbol, dtoName, dtoNamespace);

            string controllerName = symbol.Name.Replace("Controller", "");
            IOCExtension.AddIOCClassRegistration(iocRegistration, symbol, dtoName, controllerName);

            usingsForIOC.Add(dtoNamespace);
            string controllerNamespace = symbol.GetFullNamespace();
            usingsForIOC.Add(controllerNamespace);
        }
    }

    internal static IEnumerable<INamedTypeSymbol> Get(IEnumerable<INamedTypeSymbol> candidateClasses)
    {
        IEnumerable<INamedTypeSymbol> controllerClasses = candidateClasses.Where(x =>
                                        x.GetAttributes()
                                        .Any(ad => ad.AttributeClass?.Name == EnableHateoasAttributeHelper.FileName));
        return controllerClasses;
    }

    private static (string, string) GetTypeFromAttribute(INamedTypeSymbol symbol)
    {
        AttributeData attr = symbol
            .GetAttributes()
            .FirstOrDefault(ad => ad.AttributeClass?.Name == EnableHateoasAttributeHelper.FileName);

        if (attr == null || attr.ConstructorArguments.Length != 1)
        {
            return (string.Empty, string.Empty);

        }

        var typeArg = attr.ConstructorArguments[0].Value as ITypeSymbol;
        return (typeArg.Name, typeArg.ContainingNamespace.ToDisplayString());
    }

    private static void AddControllerHateoasClassToSource(
        SourceProductionContext spc,
        INamedTypeSymbol symbol,
        string dtoName,
        string dtoNamespace)
    {
        string controllerNamespace = symbol.GetFullNamespace();
        string controllerName = symbol.Name.Replace("Controller", "");

        var sb = new StringBuilder();
        AddUsings(dtoNamespace, controllerNamespace, sb);

        sb.AppendLine("namespace GeneratedHateoas");
        sb.AppendLine("{");
        sb.AppendLine($"  public class {controllerName}HateoasMeta(IHateoasFactory hateoas) : IHateoas<{symbol.Name},{dtoName}>");
        sb.AppendLine("   {");
        sb.AppendLine($"    private readonly string _constructorName = \"{controllerName}\";");
        sb.AppendLine("");

        AddCreateCollectionResponseMethod(dtoName, sb);

        sb.AppendLine("");
        AddCreateResponseMethod(dtoName, sb);
        sb.AppendLine("  }");
        sb.AppendLine("}");

        spc.AddSource($"{controllerName}_Hateoas.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }

    private static void AddCreateResponseMethod(string dtoName, StringBuilder sb)
    {
        sb.AppendLine($"    public Resource<{dtoName}> CreateResponse({dtoName} item, List<ControllerAction<{dtoName}, object>> itemActions)");
        sb.AppendLine("     {");
        sb.AppendLine($"        Resource <{dtoName}> response = hateoas.CreateResponse<{dtoName}, object>(");
        sb.AppendLine("                                                                 _constructorName,");
        sb.AppendLine("                                                                 item,");
        sb.AppendLine("                                                                 itemActions);");
        sb.AppendLine("         return response;");
        sb.AppendLine("     }");
    }

    private static void AddCreateCollectionResponseMethod(string dtoName, StringBuilder sb)
    {
        sb.AppendLine($"    public CollectionResource<{dtoName}> CreateCollectionResponse(IEnumerable<{dtoName}> items, List<ControllerAction> listActions, List<ControllerAction<{dtoName}, object>> itemActions)");
        sb.AppendLine("     {");
        sb.AppendLine($"        CollectionResource <{dtoName}> collectionResponse = hateoas.CreateCollectionResponse<{dtoName}, object>(");
        sb.AppendLine("                                                                 _constructorName,");
        sb.AppendLine("                                                                 items,");
        sb.AppendLine("                                                                 listActions,");
        sb.AppendLine("                                                                 itemActions);");
        sb.AppendLine("         return collectionResponse;");
        sb.AppendLine("     }");
    }

    private static void AddUsings(string dtoNamespace, string controllerNamespace, StringBuilder sb)
    {
        sb.AppendLine("using HateoasLib.Interfaces;");
        sb.AppendLine("using HateoasLib.Models;");
        sb.AppendLine("using HateoasLib.Models.ResponseModels;");
        sb.AppendLine($"using {controllerNamespace};");
        sb.AppendLine($"using {dtoNamespace};");
    }
}
