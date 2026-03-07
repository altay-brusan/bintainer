using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Movements.GetMovements;

public sealed record GetMovementsQuery(
    string? Action,
    Guid? ComponentId,
    string? Q,
    int Page,
    int PageSize) : IQuery<MovementsPagedResponse>;

public sealed record MovementsPagedResponse(
    int TotalCount,
    int Page,
    int PageSize,
    List<MovementItemResponse> Items);

public sealed record MovementItemResponse(
    Guid Id,
    DateTime Date,
    Guid ComponentId,
    string? ComponentPartNumber,
    string Action,
    int Quantity,
    Guid? CompartmentId,
    string? CompartmentLabel,
    Guid? SourceCompartmentId,
    string? SourceCompartmentLabel,
    string? StorageUnitName,
    Guid UserId,
    string? UserName,
    string? Notes);
