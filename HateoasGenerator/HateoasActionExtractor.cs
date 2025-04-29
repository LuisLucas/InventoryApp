using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HateoasGenerator
{
    public class HateoasAction
    {
        public string Name;
        public string Route;
        public string HttpMethod;
    };

    public static class HateoasActionExtractor
    {
        public static IEnumerable<HateoasAction> ExtractActions(INamedTypeSymbol controllerSymbol)
        {
            var controllerName = controllerSymbol.Name.Replace("Controller", "");
            var baseRoute = GetRouteTemplate(controllerSymbol) ?? $"api/{controllerName}";

            foreach (var method in controllerSymbol.GetMembers().OfType<IMethodSymbol>())
            {
                foreach (var attr in method.GetAttributes())
                {
                    var attrClass = attr.AttributeClass;
                    if (attrClass == null) continue;

                    var attrName = attrClass.Name;

                    if (!attrName.StartsWith("Http")) continue;

                    /*var httpVerb = attrName switch
                    {
                        "HttpGetAttribute" => "GET",
                        "HttpPostAttribute" => "POST",
                        "HttpPutAttribute" => "PUT",
                        "HttpDeleteAttribute" => "DELETE",
                        "HttpPatchAttribute" => "PATCH",
                        _ => null
                    };*/

                    var httpVerb = "";
                    switch (attrName)
                    {
                        case "HttpGetAttribute":
                            httpVerb = "GET";
                            break;
                        case "HttpPostAttribute":
                            httpVerb = "POST";
                            break;
                        case "HttpPutAttribute":
                            httpVerb = "PUT";
                            break;
                        case "HttpDeleteAttribute":
                            httpVerb = "DELETE";
                            break;
                        case "HttpPatchAttribute":
                            httpVerb = "PATCH";
                            break;
                        default:
                            httpVerb = "";
                            break;
                    }

                    if (httpVerb == null) continue;

                    var methodRoute = attr.ConstructorArguments.Length > 0
                        ? attr.ConstructorArguments[0].Value?.ToString()
                        : null;

                    var fullRoute = CombineRoutes(baseRoute, methodRoute);
                    yield return new HateoasAction() { Name = method.Name, Route = fullRoute, HttpMethod = httpVerb };
                }
            }
        }

        private static string GetRouteTemplate(INamedTypeSymbol symbol)
        {
            var routeAttr = symbol.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.Name == "RouteAttribute");

            return routeAttr?.ConstructorArguments.FirstOrDefault().Value?.ToString();
        }

        private static string CombineRoutes(string baseRoute, string methodRoute)
        {
            if (string.IsNullOrWhiteSpace(methodRoute)) return baseRoute;
            return $"{baseRoute.TrimEnd('/')}/{methodRoute.TrimStart('/')}";
        }
    }
}
