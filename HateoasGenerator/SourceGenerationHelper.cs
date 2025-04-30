namespace HateoasGenerator;

public static class SourceGenerationHelper
{
    public const string Attribute = @"
namespace HateoasLib.EnumGenerators
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

    public const string InterfaceClass = @"
using HateoasLib.Models.ResponseModels;
using HateoasLib.Models;

namespace HateoasLib.Interfaces;

public interface IHateoasMeta<T, R>
{
    CollectionResource<R> CreateCollectionResponse(IEnumerable<R> items,
        List<ControllerAction> listActions,
        List<ControllerAction<R, object>> itemActions);
}";

    public const string CollectionResouceClass = @"
namespace HateoasLib.Models.ResponseModels;

public class CollectionResource<T>
{
    public List<Resource<T>> Items { get; set; }

    public List<Link> Links { get; set; }
}";

    public const string ResourceClass = @"
namespace HateoasLib.Models.ResponseModels;

public class Resource<T>
{
    public T Item { get; set; }

    public List<Link> Links { get; set; }
}";

    public const string LinkClass = @"
namespace HateoasLib.Models.ResponseModels;
public record class Link(string Href, string Rel, string Method);";

    public const string ControllerActionRecords = @"
namespace HateoasLib.Models;

public record ControllerAction(string action, object? values, string rel, string method);

public record ControllerAction<T, R>(string action, Tuple<string, Func<T, R>> values, string rel, string method);";
}
