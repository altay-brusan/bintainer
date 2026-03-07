using FluentValidation;

namespace Bintainer.Modules.Users.Application.Users.UpdatePreferences;

internal sealed class UpdatePreferencesCommandValidator : AbstractValidator<UpdatePreferencesCommand>
{
    public UpdatePreferencesCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Currency).NotEmpty().MaximumLength(10);
    }
}
