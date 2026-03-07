using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Users.Application.Users.UpdateProfile;

public sealed record UpdateProfileCommand(Guid UserId, string FirstName, string LastName) : ICommand;
