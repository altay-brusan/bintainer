using Bintainer.Common.Domain;

namespace Bintainer.Modules.Catalog.Domain.BomImports;

public sealed class BomImportedDomainEvent(Guid bomImportId, string fileName, int lineCount) : DomainEvent
{
    public Guid BomImportId { get; init; } = bomImportId;
    public string FileName { get; init; } = fileName;
    public int LineCount { get; init; } = lineCount;
}
