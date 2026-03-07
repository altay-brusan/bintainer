using FluentValidation;

namespace Bintainer.Modules.Inventory.Application.Components.CreateComponent;

internal sealed class CreateComponentCommandValidator : AbstractValidator<CreateComponentCommand>
{
    public CreateComponentCommandValidator()
    {
        RuleFor(x => x.PartNumber).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ManufacturerPartNumber).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.DetailedDescription).MaximumLength(2000);
        RuleFor(x => x.ImageUrl).MaximumLength(500);
        RuleFor(x => x.Url).MaximumLength(500);
        RuleFor(x => x.Provider).MaximumLength(50);
        RuleFor(x => x.ProviderPartNumber).MaximumLength(200);
        RuleFor(x => x.Tags).MaximumLength(500);
        RuleFor(x => x.Manufacturer).MaximumLength(200);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0).When(x => x.UnitPrice.HasValue);
        RuleFor(x => x.LowStockThreshold).GreaterThanOrEqualTo(0);
    }
}
