using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace HateoasGenerator;

[Generator]
public class HateoasGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        AddAttributeClassToSource(context);
        AddInterfaceToSource(context);

        IncrementalValuesProvider<INamedTypeSymbol> classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (s, _) => IsCandidateClass(s),
                transform: (ctx, _) => GetSemanticTarget(ctx))
            .Where(symbol => symbol != null);

        IncrementalValueProvider<(Compilation Left, System.Collections.Immutable.ImmutableArray<INamedTypeSymbol> Right)> compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndClasses, (spc, source) =>
        {
            (Compilation compilation, System.Collections.Immutable.ImmutableArray<INamedTypeSymbol> classes) = source;

            var registration = new Dictionary<string, string>();
            foreach (INamedTypeSymbol symbol in classes.Distinct())
            {
                string dtoType = GetTypeFromAttribute(symbol);
                if (string.IsNullOrEmpty(dtoType))
                {
                    continue;
                }

                string controllerName = symbol.Name.Replace("Controller", "");
                AddControllerHateoasClassToSource(spc, symbol, dtoType, controllerName);

                registration.Add(symbol.Name, $"            services.AddScoped<IHateoasMeta<{symbol.Name}, {dtoType}>, {controllerName}HateoasMeta>();");

            }

            AddIOCExtensionMethodToSource(spc, registration);
        });
    }

    private static void AddIOCExtensionMethodToSource(SourceProductionContext spc, Dictionary<string, string> registration)
    {
        var newSb = new StringBuilder();
        newSb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        newSb.AppendLine("using GeneratedHateoas;");
        newSb.AppendLine("using InventoryApi.Controllers;"); // TODO: REMOVE HARDCODED USING STRINGS
        newSb.AppendLine("using HateoasLib.Myclasses;");
        newSb.AppendLine("namespace GeneratedHateoas");
        newSb.AppendLine("{");
        newSb.AppendLine("    public static class HateoasRegistration");
        newSb.AppendLine("    {");
        newSb.AppendLine("        public static IServiceCollection AddHateoasProviders(this IServiceCollection services)");
        newSb.AppendLine("        {");

        foreach (var regist in registration)
        {
            newSb.AppendLine(regist.Value);
        }

        newSb.AppendLine("            return services;");
        newSb.AppendLine("        }");
        newSb.AppendLine("    }");
        newSb.AppendLine("}");

        spc.AddSource("HateoasRegistration.g.cs", SourceText.From(newSb.ToString(), Encoding.UTF8));
    }

    private static void AddControllerHateoasClassToSource(SourceProductionContext spc, INamedTypeSymbol symbol, string dtoType, string controllerName)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using InventoryApi.Hateoas;"); // TODO: REMOVE HARDCODED USING STRINGS
        sb.AppendLine("using InventoryAPI.Application.Products;");
        sb.AppendLine("using HateoasLib.Myclasses;");
        sb.AppendLine("using InventoryApi.Controllers;");
        sb.AppendLine("using InventoryApi.Hateoas;");
        sb.AppendLine("namespace GeneratedHateoas");
        sb.AppendLine("{");
        sb.AppendLine($"  public class {controllerName}HateoasMeta(IHateoas hateoas) : IHateoasMeta<{symbol.Name},{dtoType}>");
        sb.AppendLine("   {");
        sb.AppendLine($"    private readonly string ConstructorName = \"{controllerName}\";");
        sb.AppendLine($"    public CollectionResource<{dtoType}> CreateCollectionResponse(IEnumerable<{dtoType}> items, List<ControllerAction> listActions, List<ControllerAction<{dtoType}, object>> itemActions)");
        sb.AppendLine("     {                                                                                                             ");
        sb.AppendLine($"        CollectionResource <{dtoType}> collectionResponse = hateoas.CreateCollectionResponse<{dtoType}, object>(");
        sb.AppendLine("                                                                 ConstructorName,                                 ");
        sb.AppendLine("                                                                 items,                                            ");
        sb.AppendLine("                                                                 listActions,                                      ");
        sb.AppendLine("                                                                 itemActions);                                     ");
        sb.AppendLine("         return collectionResponse;                        ");
        sb.AppendLine("     }                                                                                                             ");
        sb.AppendLine("  }");
        sb.AppendLine("}");

        spc.AddSource($"{controllerName}_Hateoas.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }

    private static string GetTypeFromAttribute(INamedTypeSymbol symbol)
    {
        var attr = symbol.GetAttributes().FirstOrDefault(ad => ad.AttributeClass?.Name == "EnableHateoasAttribute");
        if (attr == null || attr.ConstructorArguments.Length != 1)
        {
            return string.Empty;
        }
        var dtoType = attr.ConstructorArguments[0].Value?.ToString();
        return dtoType;
    }

    private static void AddInterfaceToSource(IncrementalGeneratorInitializationContext context) =>
                context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
                                                    "IHateoasMeta.g.cs",
                                                    SourceText.From(SourceGenerationHelper.InterfaceClass, Encoding.UTF8)));

    private static void AddAttributeClassToSource(IncrementalGeneratorInitializationContext context) => context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
                                                                "EnumExtensionsAttribute.g.cs",
                                                                SourceText.From(SourceGenerationHelper.Attribute, Encoding.UTF8)));

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
