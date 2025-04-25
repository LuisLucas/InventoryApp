namespace InventoryApi.Hateoas;

public record ControllerAction(string action, object? values, string rel, string method);

public record ControllerAction<T, R>(string action, Tuple<string, Func<T, R>> values, string rel, string method);
