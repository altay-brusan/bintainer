using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Domain.Movements;

public sealed class Movement : Entity
{
    private Movement() { }

    public DateTime Date { get; private set; }
    public Guid ComponentId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public Guid? CompartmentId { get; private set; }
    public Guid? SourceCompartmentId { get; private set; }
    public Guid UserId { get; private set; }
    public string? Notes { get; private set; }

    public static Movement Create(
        Guid componentId,
        string action,
        int quantity,
        Guid? compartmentId,
        Guid? sourceCompartmentId,
        Guid userId,
        string? notes)
    {
        return new Movement
        {
            Id = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            ComponentId = componentId,
            Action = action,
            Quantity = quantity,
            CompartmentId = compartmentId,
            SourceCompartmentId = sourceCompartmentId,
            UserId = userId,
            Notes = notes
        };
    }
}
