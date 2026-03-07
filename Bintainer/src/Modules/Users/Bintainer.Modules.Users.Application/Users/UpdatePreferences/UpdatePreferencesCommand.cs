using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Users.Application.Users.UpdatePreferences;

public sealed record UpdatePreferencesCommand(Guid UserId, string Currency) : ICommand;
