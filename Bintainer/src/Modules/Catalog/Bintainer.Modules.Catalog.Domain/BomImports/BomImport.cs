using Bintainer.Common.Domain;

namespace Bintainer.Modules.Catalog.Domain.BomImports;

public sealed class BomImport : Entity
{
    private BomImport() { }

    public string FileName { get; private set; } = string.Empty;
    public int TotalLines { get; private set; }
    public int MatchedCount { get; private set; }
    public int NewCount { get; private set; }
    public decimal TotalValue { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime Date { get; private set; }

    public static BomImport Create(
        string fileName,
        int totalLines,
        int matchedCount,
        int newCount,
        decimal totalValue,
        Guid userId)
    {
        return new BomImport
        {
            Id = Guid.NewGuid(),
            FileName = fileName,
            TotalLines = totalLines,
            MatchedCount = matchedCount,
            NewCount = newCount,
            TotalValue = totalValue,
            UserId = userId,
            Date = DateTime.UtcNow
        };
    }
}
