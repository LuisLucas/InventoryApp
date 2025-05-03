using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace HateoasGenerator;
internal static class IOCExtension
{
    internal static void AddIOCClassRegistration(Dictionary<string, string> registration, INamedTypeSymbol symbol, string dtoType, string controllerName) => registration.Add(symbol.Name, $"            services.AddScoped<IHateoas<{symbol.Name}, {dtoType}>, {controllerName}HateoasMeta>();");

    internal static void AddIOCExtensionMethodToSource(SourceProductionContext spc, Dictionary<string, string> registration, List<string> usingsForIOC)
    {
        var newSb = new StringBuilder();
        newSb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        newSb.AppendLine("using GeneratedHateoas;");
        newSb.AppendLine("using HateoasLib.Interfaces;");
        newSb.AppendLine("using HateoasLib.Hateoas;");
        foreach (var usingNameSpace in usingsForIOC)
        {
            newSb.AppendLine($"using {usingNameSpace};");
        }
        newSb.AppendLine("namespace GeneratedHateoas");
        newSb.AppendLine("{");
        newSb.AppendLine("    public static class HateoasRegistration");
        newSb.AppendLine("    {");
        newSb.AppendLine("        public static IServiceCollection AddHateoasProviders(this IServiceCollection services)");
        newSb.AppendLine("        {");
        newSb.AppendLine("              services.AddTransient<IHateoasFactory, HateoasFactory>();");

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
}
