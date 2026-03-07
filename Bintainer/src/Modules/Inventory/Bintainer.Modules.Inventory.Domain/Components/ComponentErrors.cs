using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Domain.Components;

public static class ComponentErrors
{
    public static Error NotFound(Guid componentId) =>
        Error.NotFound("Components.NotFound", $"The component with Id '{componentId}' was not found.");
}
