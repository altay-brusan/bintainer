using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Users.Application.Users.GetPreferences;

public sealed record GetPreferencesQuery(Guid UserId) : IQuery<PreferencesResponse>;

public sealed record PreferencesResponse(string Currency);
