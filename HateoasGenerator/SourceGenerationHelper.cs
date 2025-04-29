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
namespace HateoasLib.Myclasses
{
    using InventoryApi.Hateoas;

    public interface IHateoasMeta<T, R>
    {
        CollectionResource<R> CreateCollectionResponse(IEnumerable<R> items,
            List<ControllerAction> listActions,
            List<ControllerAction<R, object>> itemActions);
    }
}";
}
