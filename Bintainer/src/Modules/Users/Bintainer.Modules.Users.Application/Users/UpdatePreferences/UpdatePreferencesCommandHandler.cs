using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Users.Application.Abstractions.Data;
using Bintainer.Modules.Users.Domain.Users;

namespace Bintainer.Modules.Users.Application.Users.UpdatePreferences;

internal sealed class UpdatePreferencesCommandHandler(
    IUserPreferenceRepository preferenceRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdatePreferencesCommand>
{
    public async Task<Result> Handle(UpdatePreferencesCommand request, CancellationToken cancellationToken)
    {
        var preference = await preferenceRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        if (preference is null)
        {
            preference = UserPreference.Create(request.UserId, request.Currency);
            preferenceRepository.Insert(preference);
        }
        else
        {
            preference.Update(request.Currency);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
