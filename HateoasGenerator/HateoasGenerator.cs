using System.Collections.Immutable;
using System.Text;
using HateoasGenerator.Attributes;
using HateoasGenerator.Hateoas;
using HateoasGenerator.HateoasFactory;
using HateoasGenerator.Helpers;
using HateoasGenerator.Interfaces;
using HateoasGenerator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace HateoasGenerator;

[Generator]
public class HateoasGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //System.Diagnostics.Debugger.Launch();

        context
            .AddAttributeToSource()
            .AddIHateoasToSource()
            .AddLinkToSource()
            .AddResourceToSource()
            .AddCollectionResourceToSource()
            .AddControllerActionToSource()
            .AddIHateoasFactoryToSource()
            .AddFactoryToSource()
            .AddGenerateLinksToSource();

        IncrementalValuesProvider<INamedTypeSymbol> classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (s, _) => IsCandidateClass(s),
                transform: (ctx, _) => GetSemanticTarget(ctx))
            .Where(symbol => symbol != null);

        IncrementalValueProvider<(Compilation Left, ImmutableArray<INamedTypeSymbol> Right)> compilationAndClasses =
            context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndClasses, (spc, source) =>
        {
            Compilation compilation = source.Left;
            IEnumerable<INamedTypeSymbol> classes = source.Right.Distinct();

            var registration = new Dictionary<string, string>();
            var usingsForIOC = new List<string>();
            foreach (INamedTypeSymbol symbol in classes)
            {
                (string dtoName,string dtoNamespace) = GetTypeFromAttribute(symbol);
                if (string.IsNullOrEmpty(dtoName) || string.IsNullOrEmpty(dtoNamespace))
                {
                    continue;
                }

                string controllerNamespace = symbol.GetFullNamespace();
                string controllerName = symbol.Name.Replace("Controller", "");

                AddControllerHateoasClassToSource(spc, symbol, dtoName, dtoNamespace, controllerName, controllerNamespace);
                IOCExtension.AddIOCClassRegistration(registration, symbol, dtoName, controllerName);

                usingsForIOC.Add(dtoNamespace);
                usingsForIOC.Add(controllerNamespace);
            }

            IOCExtension.AddIOCExtensionMethodToSource(spc, registration, usingsForIOC);
        });
    }

    

    private static void AddControllerHateoasClassToSource(
        SourceProductionContext spc,
        INamedTypeSymbol symbol,
        string dtoName,
        string dtoNamespace,
        string controllerName,
        string controllerNamespace)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using HateoasLib.Interfaces;");
        sb.AppendLine("using HateoasLib.Models;");
        sb.AppendLine("using HateoasLib.Models.ResponseModels;");
        sb.AppendLine($"using {controllerNamespace};");
        sb.AppendLine($"using {dtoNamespace};");
        sb.AppendLine("namespace GeneratedHateoas");
        sb.AppendLine("{");
        sb.AppendLine($"  public class {controllerName}HateoasMeta(IHateoasFactory hateoas) : IHateoas<{symbol.Name},{dtoName}>");
        sb.AppendLine("   {");
        sb.AppendLine($"    private readonly string ConstructorName = \"{controllerName}\";");
        sb.AppendLine($"    public CollectionResource<{dtoName}> CreateCollectionResponse(IEnumerable<{dtoName}> items, List<ControllerAction> listActions, List<ControllerAction<{dtoName}, object>> itemActions)");
        sb.AppendLine("     {");
        sb.AppendLine($"        CollectionResource <{dtoName}> collectionResponse = hateoas.CreateCollectionResponse<{dtoName}, object>(");
        sb.AppendLine("                                                                 ConstructorName,");
        sb.AppendLine("                                                                 items,");
        sb.AppendLine("                                                                 listActions,");
        sb.AppendLine("                                                                 itemActions);");
        sb.AppendLine("         return collectionResponse;");
        sb.AppendLine("     }");
        sb.AppendLine("");
        sb.AppendLine($"    public Resource<{dtoName}> CreateResponse({dtoName} item, List<ControllerAction<{dtoName}, object>> itemActions)");
        sb.AppendLine("     {");
        sb.AppendLine($"        Resource <{dtoName}> response = hateoas.CreateResponse<{dtoName}, object>(");
        sb.AppendLine("                                                                 ConstructorName,");
        sb.AppendLine("                                                                 item,");
        sb.AppendLine("                                                                 itemActions);");
        sb.AppendLine("         return response;");
        sb.AppendLine("     }");
        sb.AppendLine("  }");
        sb.AppendLine("}");

        spc.AddSource($"{controllerName}_Hateoas.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }

    private static (string,string) GetTypeFromAttribute(INamedTypeSymbol symbol)
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

    private static bool IsCandidateClass(SyntaxNode node) =>
        node is ClassDeclarationSyntax c && c.AttributeLists.Count > 0;

    private static INamedTypeSymbol GetSemanticTarget(GeneratorSyntaxContext context)
    {
        var classSyntax = (ClassDeclarationSyntax)context.Node;
        var symbol = context.SemanticModel.GetDeclaredSymbol(classSyntax) as INamedTypeSymbol;
        if (symbol == null) return null;

        var hasHateoasAttr = symbol.GetAttributes()
            .Any(attr => attr.AttributeClass?.Name.Contains("EnableHateoas") == true);

        return hasHateoasAttr ? symbol : null;
    }
}
