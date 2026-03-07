using Bintainer.Common.Domain;

namespace Bintainer.Modules.Users.Domain.Users;

public sealed class UserPreference : Entity
{
    private UserPreference() { }

    public Guid UserId { get; private set; }
    public string Currency { get; private set; } = "USD";

    public static UserPreference Create(Guid userId, string currency = "USD")
    {
        return new UserPreference
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Currency = currency
        };
    }

    public void Update(string currency)
    {
        Currency = currency;
    }
}
