using Bintainer.Common.Application.Authorization;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Catalog.Application.Abstractions.Data;
using Bintainer.Modules.Catalog.Domain.BomImports;
using Bintainer.Modules.Catalog.Domain.Components;

namespace Bintainer.Modules.Catalog.Application.BomImports.ImportBom;

internal sealed class ImportBomCommandHandler(
    IComponentRepository componentRepository,
    IBomImportRepository bomImportRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork) : ICommandHandler<ImportBomCommand, ImportBomResponse>
{
    public async Task<Result<ImportBomResponse>> Handle(ImportBomCommand request, CancellationToken cancellationToken)
    {
        int matchedCount = 0;
        int newCount = 0;
        decimal totalValue = 0;

        foreach (var line in request.Lines)
        {
            var existing = await componentRepository.GetByPartNumberAsync(line.PartNumber, cancellationToken);

            if (existing is not null)
            {
                matchedCount++;
                totalValue += (existing.UnitPrice ?? 0) * line.Quantity;
            }
            else
            {
                var component = Component.Create(
                    line.PartNumber,
                    line.PartNumber,
                    line.Description ?? line.PartNumber,
                    null, null, null, null, null, null, null, null, null, null, null, 0);

                componentRepository.Insert(component);
                newCount++;
            }
        }

        var bomImport = BomImport.Create(
            request.FileName,
            request.Lines.Count,
            matchedCount,
            newCount,
            totalValue,
            currentUserService.UserId);

        bomImportRepository.Insert(bomImport);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ImportBomResponse(
            bomImport.Id,
            request.Lines.Count,
            matchedCount,
            newCount,
            totalValue);
    }
}
