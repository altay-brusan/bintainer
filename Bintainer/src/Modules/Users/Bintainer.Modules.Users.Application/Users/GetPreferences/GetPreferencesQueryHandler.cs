using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Users.Domain.Users;

namespace Bintainer.Modules.Users.Application.Users.GetPreferences;

internal sealed class GetPreferencesQueryHandler(
    IUserPreferenceRepository preferenceRepository) : IQueryHandler<GetPreferencesQuery, PreferencesResponse>
{
    public async Task<Result<PreferencesResponse>> Handle(GetPreferencesQuery request, CancellationToken cancellationToken)
    {
        var preference = await preferenceRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        var currency = preference?.Currency ?? "USD";

        return new PreferencesResponse(currency);
    }
}
