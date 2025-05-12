using System.Collections.Immutable;
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

            IEnumerable<INamedTypeSymbol> controllerClasses = HateoasControllerClassGenerator.Get(classes);

            IEnumerable<INamedTypeSymbol> modelClasses = classes.Where(x =>
                                        x.GetAttributes()
                                        .Any(ad => ad.AttributeClass?.Name == HateoasAttribute.FileName));

            var registration = new Dictionary<string, string>();
            var usingsForIOC = new List<string>();
            HateoasControllerClassGenerator.GenerateHateoasControllerClass(spc, classes, registration, usingsForIOC);

            foreach (INamedTypeSymbol symbol in modelClasses)
            {
                Dictionary<string, List<ImmutableArray<TypedConstant>>> actionsAttributes = ExtractAttributesFromType(symbol);
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

    private void AddModelHateoasClassToSource(SourceProductionContext spc, Dictionary<string, List<ImmutableArray<TypedConstant>>> actionsAttributes, string typeName, string typeNamespace)
    {
        List<string> actonList = GetActionsFromAttributes(actionsAttributes["HateoasAttribute"]);

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
        sb.AppendLine($"    public CollectionResource<{typeName}> CreateCollectionResponse(IEnumerable<{typeName}> items, Type controller)");
        sb.AppendLine("     {");
        sb.AppendLine($"         var itemActions = new List<ControllerAction<{typeName}, object>>();");

        int idx = 0;
        foreach (ImmutableArray<TypedConstant> action in actionsAttributes["HateoasAttribute"])
        {
            string method = action[0].Value as string;
            string rel = action[1].Value as string;
            string property = action[2].Value as string;
            sb.AppendLine($"        var value{idx} = new Tuple<string, Func<{typeName}, object>>(\"{property.ToLowerInvariant()}\", new Func<{typeName}, object>((item) => item.{property}));");
            sb.AppendLine($"        itemActions.Add(new ControllerAction<{typeName}, object>(\"{method}\", value{idx}, \"{rel}\", \"{method}\"));");
            idx++;
        }

        sb.AppendLine("         var listActions = new List<ControllerAction>();");

        foreach (ImmutableArray<TypedConstant> action in actionsAttributes["HateoasListAttribute"])
        {
            string method = action[0].Value as string;
            string rel = action[1].Value as string;
                sb.AppendLine($"        listActions.Add(new ControllerAction(\"{method}\", new {{ }}, \"{rel}\", \"{method}\"));");
        }

        sb.AppendLine("");
        sb.AppendLine($"        CollectionResource<{typeName}> collectionResponse = hateoas.CreateCollectionResponse<{typeName}, object>(");
        sb.AppendLine("                                                                 controller.Name.Replace(\"Controller\", \"\"),");
        sb.AppendLine("                                                                 items,");
        sb.AppendLine("                                                                 listActions,");
        sb.AppendLine("                                                                 itemActions);");

        sb.AppendLine("         return collectionResponse;");
        sb.AppendLine("     }");
        sb.AppendLine("");
        sb.AppendLine($"    public PaginatedResource<{typeName}> CreatePaginatedResponse(IEnumerable<{typeName}> items, Type controller, int page, int pageSize, int totalNumberOfRecords)");
        sb.AppendLine("     {");
        sb.AppendLine($"         var itemActions = new List<ControllerAction<{typeName}, object>>();");

        idx = 0;
        foreach (ImmutableArray<TypedConstant> action in actionsAttributes["HateoasAttribute"])
        {
            string method = action[0].Value as string;
            string rel = action[1].Value as string;
            string property = action[2].Value as string;
            sb.AppendLine($"        var value{idx} = new Tuple<string, Func<{typeName}, object>>(\"{property.ToLowerInvariant()}\", new Func<{typeName}, object>((item) => item.{property}));");
            sb.AppendLine($"        itemActions.Add(new ControllerAction<{typeName}, object>(\"{method}\", value{idx}, \"{rel}\", \"{method}\"));");
            idx++;
        }

        sb.AppendLine("         var listActions = new List<ControllerAction>();");

        foreach (ImmutableArray<TypedConstant> action in actionsAttributes["HateoasListAttribute"])
        {
            string method = action[0].Value as string;
            string rel = action[1].Value as string;
            if(rel == "self")
            {
                sb.AppendLine($"        listActions.Add(new ControllerAction(\"{method}\", new {{ page }}, \"{rel}\", \"{method}\"));");
                sb.AppendLine($"        listActions.Add(new ControllerAction(\"{method}\", new {{ page = 1 }}, \"first\", \"first\"));");
                sb.AppendLine($"        listActions.Add(new ControllerAction(\"{method}\", new {{ page = page + 1 }}, \"next\", \"next\"));");
                sb.AppendLine($"        listActions.Add(new ControllerAction(\"{method}\", new {{ page = (int)Math.Ceiling((double)totalNumberOfRecords / pageSize)}}, \"last\", \"last\"));");
            }
            else
            {
                sb.AppendLine($"        listActions.Add(new ControllerAction(\"{method}\", new {{ }}, \"{rel}\", \"{method}\"));");
            }
        }

        sb.AppendLine("");
        sb.AppendLine("         var result = hateoas.CreatePaginatedResponse<ProductModel>(controller.Name.Replace(\"Controller\", \"\"), items, listActions, itemActions);");
        sb.AppendLine("         result.Page = page; result.PageSize = pageSize; result.TotalItems = totalNumberOfRecords;");
        sb.AppendLine("         return result;");
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

    public static Dictionary<string, List<ImmutableArray<TypedConstant>>> ExtractAttributesFromType(INamedTypeSymbol typeSymbol)
    {
        var actions = new Dictionary<string, List<ImmutableArray<TypedConstant>>>()
        {
            { "HateoasAttribute", new List<ImmutableArray<TypedConstant>>() },
            { "HateoasListAttribute", new List<ImmutableArray<TypedConstant>>() }
        };
        foreach (AttributeData attributeData in typeSymbol.GetAttributes())
        {
            string attrClassName = attributeData.AttributeClass?.Name;

            if (attrClassName is null || !attrClassName.StartsWith("Hateoas"))
            {
                continue;
            }

            if (actions.ContainsKey(attrClassName))
            {
                actions[attrClassName].Add(attributeData.ConstructorArguments);
            }
        }
        return actions;
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
