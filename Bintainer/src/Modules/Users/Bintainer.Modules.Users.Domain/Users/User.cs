using Bintainer.Common.Domain;

namespace Bintainer.Modules.Users.Domain.Users;

public sealed class User : Entity
{
    private User() { }

    public string Email { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string IdentityId { get; private set; } = string.Empty;

    public static User Create(string email, string firstName, string lastName, string identityId)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            IdentityId = identityId
        };

        user.Raise(new UserRegisteredDomainEvent(user.Id));

        return user;
    }

    public void UpdateProfile(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}
