using FluentValidation;

namespace Bintainer.Modules.Inventory.Application.Parts.UpdatePart;

internal sealed class UpdatePartCommandValidator : AbstractValidator<UpdatePartCommand>
{
    public UpdatePartCommandValidator()
    {
        RuleFor(x => x.PartId).NotEmpty();
        RuleFor(x => x.PartNumber).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ManufacturerPartNumber).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.DetailedDescription).MaximumLength(2000);
        RuleFor(x => x.ImageUrl).MaximumLength(500);
        RuleFor(x => x.Url).MaximumLength(500);
        RuleFor(x => x.Provider).MaximumLength(50);
        RuleFor(x => x.ProviderPartNumber).MaximumLength(200);
        RuleFor(x => x.Tags).MaximumLength(500);
    }
}
