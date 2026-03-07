using FluentValidation;

namespace Bintainer.Modules.Inventory.Application.BomImports.ImportBom;

internal sealed class ImportBomCommandValidator : AbstractValidator<ImportBomCommand>
{
    public ImportBomCommandValidator()
    {
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Lines).NotEmpty();
    }
}
