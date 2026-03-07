using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Reports.GetStorageUtilization;

public sealed record GetStorageUtilizationQuery() : IQuery<IReadOnlyCollection<StorageUtilizationResponse>>;

public sealed record StorageUtilizationResponse(
    Guid StorageUnitId,
    string StorageUnitName,
    int TotalCompartments,
    int OccupiedCompartments);
