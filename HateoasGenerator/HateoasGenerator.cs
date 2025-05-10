using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Reflection;
using System.Text;
using HateoasGenerator.Attributes;
using HateoasGenerator.Hateoas;
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
            .AddHateoasListAttributeToSource()
            .AddHateoasAttributeToSource()
            .AddIHateoasToSource()
            .AddIHateoasOneArgToSource()
            .AddLinkToSource()
            .AddResourceToSource()
            .AddCollectionResourceToSource()
            .AddPaginatedResourceToSource()
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

            IEnumerable<INamedTypeSymbol> controllerClasses = classes.Where(x =>
                                                    x.GetAttributes()
                                                    .Any(ad => ad.AttributeClass?.Name == EnableHateoasAttributeHelper.FileName));

            IEnumerable<INamedTypeSymbol> modelClasses = classes.Where(x =>
                                        x.GetAttributes()
                                        .Any(ad => ad.AttributeClass?.Name == HateoasAttribute.FileName));

            var registration = new Dictionary<string, string>();
            var usingsForIOC = new List<string>();
            foreach (INamedTypeSymbol symbol in controllerClasses)
            {
                (string dtoName, string dtoNamespace) = GetTypeFromAttribute(symbol);
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

            foreach (INamedTypeSymbol symbol in modelClasses)
            {
                List<ImmutableArray<TypedConstant>> actionsAttributes = ExtractAttributesFromType(symbol);
                if (actionsAttributes == null || !actionsAttributes.Any())
                {
                    continue;
                }

                string typeNamespace = symbol.GetFullNamespace();
                string typeName = symbol.Name;

                AddModelHateoasClassToSource(spc, actionsAttributes, typeName, typeNamespace);
                IOCExtension.AddIOCClassRegistration(registration, typeName);

                //usingsForIOC.Add(dtoNamespace);
                usingsForIOC.Add(typeNamespace);
            }

            IOCExtension.AddIOCExtensionMethodToSource(spc, registration, usingsForIOC);
        });
    }

    private void AddModelHateoasClassToSource(SourceProductionContext spc, List<ImmutableArray<TypedConstant>> actionsAttributes, string typeName, string typeNamespace)
    {
        List<string> actonList = GetActionsFromAttributes(actionsAttributes);

        var sb = new StringBuilder();
        sb.AppendLine("using HateoasLib.Interfaces;");
        sb.AppendLine("using HateoasLib.Models;");
        sb.AppendLine("using HateoasLib.Models.ResponseModels;");
        sb.AppendLine($"using {typeNamespace};");
        sb.AppendLine("namespace GeneratedHateoas");
        sb.AppendLine("{");
        sb.AppendLine($"  public class {typeName}HateoasMeta(IHateoasFactory hateoas) : IHateoas<{typeName}>");
        sb.AppendLine("   {");
        sb.AppendLine("");
        sb.AppendLine($"    public Resource<{typeName}> CreateResponse({typeName} item, Type controller)");
        sb.AppendLine("     {");
        sb.AppendLine("         var itemActions = new List<ControllerAction>();");

        foreach (string controllerAction in actonList)
        {
            sb.AppendLine($"        itemActions.Add({controllerAction});");
        }

        sb.AppendLine($"        Resource <{typeName}> response = hateoas.CreateResponse<{typeName}>(");
        sb.AppendLine("                                                                 controller.Name.Replace(\"Controller\", \"\"),");
        sb.AppendLine("                                                                 item,");
        sb.AppendLine("                                                                 itemActions);");
        sb.AppendLine("         return response;");
        sb.AppendLine("     }");
        sb.AppendLine("");
        sb.AppendLine($"    public PaginatedResource<{typeName}> CreatePaginatedResponse(IEnumerable<{typeName}> items, Type controller, int page, int pageSize, int totalNumberOfRecords)");
        sb.AppendLine("     {");
        sb.AppendLine($"         var itemActions = new List<ControllerAction<{typeName}, object>>();");

        var idx = 0;
        foreach (ImmutableArray<TypedConstant> action in actionsAttributes)
        {
            string method = action[0].Value as string;
            string rel = action[1].Value as string;
            string property = action[2].Value as string;
            sb.AppendLine($"        var value{idx} = new Tuple<string, Func<{typeName}, object>>(\"{property.ToLowerInvariant()}\", new Func<{typeName}, object>((item) => item.{property}));");
            sb.AppendLine($"        itemActions.Add(new ControllerAction<{typeName}, object>(\"{method}\", value{idx}, \"{rel}\", \"{method}\"));");
            idx++;
        }

        sb.AppendLine("         var listActions = new List<ControllerAction>();");
        sb.AppendLine($"        listActions.Add(new ControllerAction(\"Get\", new {{ page }}, \"self\", \"Get\"));");
        sb.AppendLine($"        listActions.Add(new ControllerAction(\"Get\", new {{ page }}, \"first\", \"first\"));");
        sb.AppendLine($"        listActions.Add(new ControllerAction(\"Get\", new {{ page = page + 1 }}, \"next\", \"next\"));");
        sb.AppendLine($"        listActions.Add(new ControllerAction(\"Get\", new {{ page = (int)Math.Ceiling((double)totalNumberOfRecords / pageSize)}}, \"last\", \"last\"));");
        sb.AppendLine("");
        sb.AppendLine("         var result = hateoas.CreateCollectionResponse<ProductModel, object>(controller.Name.Replace(\"Controller\", \"\"), items, listActions, itemActions);");
        sb.AppendLine("         return new HateoasLib.Models.ResponseModels.PaginatedResource<ProductModel>(){Items = result.Items, Links = result.Links, Page = page, PageSize = pageSize, TotalItems = totalNumberOfRecords};");
        sb.AppendLine("     }");
        sb.AppendLine("");
        sb.AppendLine("  }");
        sb.AppendLine("}");

        spc.AddSource($"{typeName}_Hateoas.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }

    private static List<string> GetActionsFromAttributes(List<ImmutableArray<TypedConstant>> actionsAttributes)
    {
        List<string> sb = [];
        foreach (ImmutableArray<TypedConstant> action in actionsAttributes)
        {
            string method = action[0].Value as string;
            string rel = action[1].Value as string;
            string property = action[2].Value as string;
            sb.Add($"new ControllerAction(\"{method}\", new {{ {property.ToLowerInvariant()} = item.{property} }}, \"{rel}\", \"{method}\")");
        }
        return sb;
    }

    public static List<ImmutableArray<TypedConstant>> ExtractAttributesFromType(INamedTypeSymbol typeSymbol)
    {
        var actions = new List<ImmutableArray<TypedConstant>>();
        foreach (AttributeData attributeData in typeSymbol.GetAttributes())
        {
            string attrClassName = attributeData.AttributeClass?.Name;

            if (attrClassName is null || !attrClassName.StartsWith("Hateoas"))
            {
                continue;
            }

            actions.Add(attributeData.ConstructorArguments);
        }
        return actions;
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
        sb.AppendLine($"    private readonly string _constructorName = \"{controllerName}\";");
        sb.AppendLine("");
        sb.AppendLine($"    public CollectionResource<{dtoName}> CreateCollectionResponse(IEnumerable<{dtoName}> items, List<ControllerAction> listActions, List<ControllerAction<{dtoName}, object>> itemActions)");
        sb.AppendLine("     {");
        sb.AppendLine($"        CollectionResource <{dtoName}> collectionResponse = hateoas.CreateCollectionResponse<{dtoName}, object>(");
        sb.AppendLine("                                                                 _constructorName,");
        sb.AppendLine("                                                                 items,");
        sb.AppendLine("                                                                 listActions,");
        sb.AppendLine("                                                                 itemActions);");
        sb.AppendLine("         return collectionResponse;");
        sb.AppendLine("     }");
        sb.AppendLine("");
        sb.AppendLine($"    public Resource<{dtoName}> CreateResponse({dtoName} item, List<ControllerAction<{dtoName}, object>> itemActions)");
        sb.AppendLine("     {");
        sb.AppendLine($"        Resource <{dtoName}> response = hateoas.CreateResponse<{dtoName}, object>(");
        sb.AppendLine("                                                                 _constructorName,");
        sb.AppendLine("                                                                 item,");
        sb.AppendLine("                                                                 itemActions);");
        sb.AppendLine("         return response;");
        sb.AppendLine("     }");
        sb.AppendLine("  }");
        sb.AppendLine("}");

        spc.AddSource($"{controllerName}_Hateoas.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
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

    private static bool IsCandidateClass(SyntaxNode node) =>
        (node is ClassDeclarationSyntax c && c.AttributeLists.Count > 0) || (node is RecordDeclarationSyntax r && r.AttributeLists.Count > 0);

    private static INamedTypeSymbol GetSemanticTarget(GeneratorSyntaxContext context)
    {
        if (context.Node is ClassDeclarationSyntax)
        {
            var classSyntax = (ClassDeclarationSyntax)context.Node;
            if (context.SemanticModel.GetDeclaredSymbol(classSyntax) is not INamedTypeSymbol symbol)
            {
                return null;
            }
            bool hasHateoasAttr = symbol.GetAttributes()
                .Any(attr => attr.AttributeClass?.Name.Contains("EnableHateoas") == true
                               || attr.AttributeClass?.Name.Contains("Hateoas") == true);

            return hasHateoasAttr ? symbol : null;
        }
        else if (context.Node is RecordDeclarationSyntax classSyntax)
        {
            if (context.SemanticModel.GetDeclaredSymbol(classSyntax) is not INamedTypeSymbol symbol)
            {
                return null;
            }

            bool hasHateoasAttr = symbol.GetAttributes()
                .Any(attr => attr.AttributeClass?.Name.Contains("Hateoas") == true
                               || attr.AttributeClass?.Name.Contains("Hateoas") == true);

            return hasHateoasAttr ? symbol : null;
        }

        return null;
    }
}
